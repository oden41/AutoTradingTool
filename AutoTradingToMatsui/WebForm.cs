using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Utility;

namespace AutoTradingToMatsui
{
    public partial class WebForm : Form
    {
        const string loginURL = @"http://pocket.matsui.co.jp/member/servlet/Login?uid=NULLGWDOCOMO";
        const string mainURL = @"https://pocket.matsui.co.jp/member/servlet/Generic";
        private string ID { get { return ConfigurationManager.AppSettings["ID"]; } }//ログインID
        private string PASSWORD { get { return ConfigurationManager.AppSettings["PASSWORD"]; } }//ログインパスワード
        private string TRADEPASS { get { return ConfigurationManager.AppSettings["TRADEPASSWORD"]; } }//取引パス
        private double ONEDAYTRADERATIO = 0.3;//1日に投資する最大金額(全資産比)

        //銘柄コード -> [銘柄名,保有株数,取得平均,評価単価]
        private Dictionary<string, object[]> portfolio;
        //データベース
        private DataBase db;
        //<戦略sql,保有日数>のペア
        private List<KeyValuePair<string, int>> strategyList;
        //売買予定リスト
        private List<object[]> tradeList;
        private List<string> sellList;
        private double CashAmount = -1;
        private double AllAssetAmount = -1;

        public WebForm()
        {
            InitializeComponent();
            db = new DataBase();

            strategyList = new List<KeyValuePair<string, int>>();
            //GPR4.8,上昇局面タイプだがリーマンショックから早く回復
            strategyList.Add(new KeyValuePair<string, int>("../../SQL/20170530.sql", 10));
            //GPR4近い，上昇局面に強い
            strategyList.Add(new KeyValuePair<string, int>("../../SQL/20170612.sql", 20));

            tradeList = new List<object[]>();
            sellList = new List<string>();
        }

        private void WebForm_Load(object sender, EventArgs e)
        {
            Show();
        }

        /// <summary>
        /// ログインし，トップページに推移する
        /// </summary>
        public void Login()
        {
            webBrowser.NavigateAndWait(loginURL);
            var htmlCollection = webBrowser.Document.GetElementsByTagName("input");
            htmlCollection.GetElementsByName("clientCD")[0].InnerText = ID;
            htmlCollection.GetElementsByName("passwd")[0].InnerText = PASSWORD;
            htmlCollection[8].InvokeMember("click"); // フォームのサブミット
            Wait();
        }

        /// <summary>
        /// トップページに遷移する
        /// </summary>
        public void GoToMain()
        {
            webBrowser.NavigateAndWait(mainURL);
            if (webBrowser.Document.Body.InnerText.Contains("接続がキャンセル"))
                Login();
        }

        /// <summary>
        /// メインページの状態から現物買の注文まで行う
        /// </summary>
        /// <param name="code"></param>
        /// <param name="stockUnit"></param>
        /// <param name="price"></param>
        /// <param name="isNariyuki"></param>
        /// <param name="execCond">執行条件</param>
        /// <param name="validDT">有効期間</param>
        /// <returns></returns>
        public bool BuyOrder(string code, int stockUnit, double price, bool isNariyuki, int execCond = 0, int validDT = 0)
        {
            try
            {
                if (webBrowser.Url?.AbsoluteUri != mainURL)
                    Login();
                //株式取引選択
                var topCollection = webBrowser.Document.GetElementsByTagName("a");
                topCollection.Cast<HtmlElement>().Where(e => e.InnerText == "株式取引").First().InvokeMember("click");
                Wait();

                //現物買選択
                var buyCollection = webBrowser.Document.GetElementsByTagName("a");
                buyCollection.Cast<HtmlElement>().Where(e => e.InnerText == "現物買").First().InvokeMember("click");
                Wait();

                //銘柄コード入力
                var selectStockCollection = webBrowser.Document.GetElementsByTagName("input");
                selectStockCollection.GetElementsByName("dscrCD")[0].InnerText = code;
                selectStockCollection.Cast<HtmlElement>().Where(e => e.Name == "CONFIRM").First().InvokeMember("click");
                Wait();

                //注文画面
                var orderStockCollection = webBrowser.Document.GetElementsByTagName("input");
                //株数
                orderStockCollection.GetElementsByName("orderNominal")[0].InnerText = stockUnit.ToString();
                if (!isNariyuki)
                {
                    //指値
                    orderStockCollection.GetElementsByName("limitPrice")[0].InnerText = price.ToString();
                }
                else
                {
                    //成行
                    orderStockCollection.Cast<HtmlElement>().Where(e => e.Name == "marketPrice").First().InvokeMember("click");
                }
                //執行条件
                webBrowser.Document.GetElementsByTagName("select").GetElementsByName("execCondCD")[0].SetAttribute("value", execCond.ToString());
                //有効期間
                orderStockCollection.GetElementsByName("validDT")[validDT].InvokeMember("click");
                orderStockCollection.Cast<HtmlElement>().Where(e => e.Name == "CONFIRM").First().InvokeMember("click");
                Wait();

                //確認?
                if (webBrowser.Document.Body.InnerText.Contains("時間外エラー"))
                    throw new Exception("時間外エラー");

                //暗証パス入力
                var doc = webBrowser.Document;
                webBrowser.Document.GetElementsByTagName("input").GetElementsByName("submitPassword")[0].InnerText = TRADEPASS;
                webBrowser.Document.GetElementsByTagName("input").Cast<HtmlElement>().Where(e => e.Name == "CONFIRM").First().InvokeMember("click");
                Wait();

                //お金があるか
                if (webBrowser.Document.Body.InnerText.Contains("余力エラー"))
                    throw new Exception("余力エラー");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                GoToMain();
                return false;
            }

            return true;
        }



        /// <summary>
        /// メインページの状態から現物売の注文まで行う
        /// </summary>
        /// <param name="code"></param>
        /// <param name="stockUnit"></param>
        /// <param name="price"></param>
        /// <param name="isNariyuki"></param>
        /// <param name="execCond">執行条件 0：なし，1：指成，2：寄付，3：引け</param>
        /// <param name="validDT">有効期間</param>
        /// <returns></returns>
        public bool SellOrder(string code, int stockUnit, double price, bool isNariyuki, int execCond = 0, int validDT = 0)
        {
            try
            {
                if (webBrowser.Url?.AbsoluteUri != mainURL)
                    Login();
                //株式取引選択
                var topCollection = webBrowser.Document.GetElementsByTagName("a");
                topCollection.Cast<HtmlElement>().Where(e => e.InnerText == "株式取引").First().InvokeMember("click");
                Wait();

                //現物売選択
                var buyCollection = webBrowser.Document.GetElementsByTagName("a");
                buyCollection.Cast<HtmlElement>().Where(e => e.InnerText == "現物売").First().InvokeMember("click");
                Wait();

                //銘柄コード入力
                var selectStockCollection = webBrowser.Document.GetElementsByTagName("input");
                selectStockCollection.GetElementsByName("dscrCD")[0].InnerText = code;
                selectStockCollection.Cast<HtmlElement>().Where(e => e.Name == "CONFIRM").First().InvokeMember("click");
                Wait();

                //注文画面
                var orderStockCollection = webBrowser.Document.GetElementsByTagName("input");
                //株数
                orderStockCollection.GetElementsByName("orderNominal")[0].InnerText = stockUnit.ToString();
                if (!isNariyuki)
                {
                    //指値
                    orderStockCollection.GetElementsByName("limitPrice")[0].InnerText = price.ToString();
                }
                else
                {
                    //成行
                    orderStockCollection.Cast<HtmlElement>().Where(e => e.Name == "marketPrice").First().InvokeMember("click");
                }
                //執行条件
                webBrowser.Document.GetElementsByTagName("select").GetElementsByName("execCondCD")[0].SetAttribute("value", execCond.ToString());
                //有効期間
                orderStockCollection.GetElementsByName("validDT")[validDT].InvokeMember("click");
                orderStockCollection.Cast<HtmlElement>().Where(e => e.Name == "CONFIRM").First().InvokeMember("click");
                Wait();

                //確認?
                if (webBrowser.Document.Body.InnerText.Contains("時間外エラー"))
                    throw new Exception("時間外エラー");

                //暗証パス入力
                var doc = webBrowser.Document;
                webBrowser.Document.GetElementsByTagName("input").GetElementsByName("submitPassword")[0].InnerText = TRADEPASS;
                webBrowser.Document.GetElementsByTagName("input").Cast<HtmlElement>().Where(e => e.Name == "CONFIRM").First().InvokeMember("click");
                Wait();

                //お金があるか
                if (webBrowser.Document.Body.InnerText.Contains("余力エラー"))
                    throw new Exception("余力エラー");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                GoToMain();
                return false;
            }

            return true;
        }

        /// <summary>
        /// 保有銘柄を辞書に登録するメソッド
        /// </summary>
        public bool CheckPortfolio()
        {
            try
            {
                if (webBrowser.Url?.AbsoluteUri != mainURL)
                    Login();

                if (portfolio == null)
                    portfolio = new Dictionary<string, object[]>();

                portfolio.Clear();
                //株式取引選択
                var topCollection = webBrowser.Document.GetElementsByTagName("a");
                topCollection.Cast<HtmlElement>().Where(e => e.InnerText == "株式取引").First().InvokeMember("click");
                Wait();

                //現物売選択
                var sellCollection = webBrowser.Document.GetElementsByTagName("a");
                sellCollection.Cast<HtmlElement>().Where(e => e.InnerText == "現物売").First().InvokeMember("click");
                Wait();

                //formの２番目以降に各銘柄の保有情報あり
                var portfolioCollection = webBrowser.Document.GetElementsByTagName("form");
                for (int i = 1; i < portfolioCollection.Count; i++)
                {
                    var stockInfo = portfolioCollection[i];
                    var split = stockInfo.InnerText.Trim(' ').Replace("\r\n", "\n").Split('\n');
                    var code = split[0].Split(':')[1];
                    var name = split[1];
                    var noOfStock = double.Parse(split[2].Split(':')[1].Replace("株", ""));
                    var avePrice = double.Parse(split[4].Split(':')[1].Replace("円", ""));
                    var nowPrice = double.Parse(split[5].Split(':')[1].Replace("円", ""));
                    if(!portfolio.Keys.Contains(code))
                        portfolio.Add(code, new object[] { name, noOfStock, avePrice, nowPrice });
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 約定情報を確認するメソッド
        /// TODO: 複数の約定があった場合
        /// </summary>
        public bool CheckContract()
        {
            try
            {
                if (webBrowser.Url?.AbsoluteUri != mainURL)
                    Login();

                if (portfolio == null)
                    portfolio = new Dictionary<string, object[]>();

                portfolio.Clear();
                //株式取引選択
                var topCollection = webBrowser.Document.GetElementsByTagName("a");
                topCollection.Cast<HtmlElement>().Where(e => e.InnerText == "株式取引").First().InvokeMember("click");
                Wait();

                //現物売選択
                var sellCollection = webBrowser.Document.GetElementsByTagName("a");
                sellCollection.Cast<HtmlElement>().Where(e => e.InnerText == "約定照会").First().InvokeMember("click");
                Wait();


                var text = webBrowser.Document.Body.InnerText;
                var split = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < split.Length; i = i + 11)
                {
                    var code = split[i + 1].Split(':')[1];
                    var name = split[i + 2];
                    var buyorsell = split[i + 3].Contains("買") ? BuySell.Buy : BuySell.Sell;
                    var unit = split[i + 4].Split(':')[1].Replace("株", "");
                    var price = split[i + 5].Split(':')[1].Replace("円", "");
                    //var posDate = tradeList.Where(arr => arr[0].ToString() == code)?.First()[2].ToString();
                    var posDate = "10";

                    var insert = $@"INSERT INTO StockContract.dbo.ContractTable 
(
Date,
CodeNum,
Price,
UnitNum,
Type,
State,
PosDate
)
VALUES
(
'{DateTime.Today.ToShortDateString()}',
'{code}',
{price},
{unit},
'{buyorsell.DisplayName()}',
'Pos',
{posDate}
)
";
                    db.ExecuteNonQuery(insert);
                }

            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// DBを見に行き，前営業日にサインが点灯しているかをチェックする
        /// 存在していたらtradeListに銘柄コードと終値を追加
        /// </summary>
        public void CheckSign(DateTime date)
        {
            var asOf = GetPrevWorkDay(date);
            //銘柄コード，終値，保有日数
            var list = new List<object[]>();
            foreach (var strategy in strategyList)
            {
                var sql = "";
                using (StreamReader sr = new StreamReader(strategy.Key, Encoding.UTF8))
                {
                    sql = sr.ReadToEnd();
                }
                var result = db.ExecuteReader(sql)?.Rows.Cast<DataRow>();
                if (result == null)
                    continue;
                var filtered = result.Where(row => DateTime.Parse(row["AsOfDate"].ToString()) == asOf).ToList();

                filtered.ForEach(row => tradeList.Add(new object[] { row["CodeNum"].ToString(), double.Parse(row["NowPrice"].ToString()), strategy.Value}));
            }
        }


        public void CheckSign()
        {
            CheckSign(DateTime.Today);
        }


        /// <summary>
        /// 資産状況をチェックするメソッド
        /// </summary>
        public bool CheckAssetAmount()
        {
            try
            {
                if (webBrowser.Url?.AbsoluteUri != mainURL)
                    Login();

                //資産状況選択
                var topCollection = webBrowser.Document.GetElementsByTagName("a");
                topCollection.Cast<HtmlElement>().Where(e => e.InnerText == "資産状況").First().InvokeMember("click");
                Wait();

                //資産状況一覧選択
                var sellCollection = webBrowser.Document.GetElementsByTagName("a");
                sellCollection.Cast<HtmlElement>().Where(e => e.InnerText == "資産状況一覧").First().InvokeMember("click");
                Wait();

                var text = webBrowser.Document.Body.InnerText;
                var split = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                CashAmount = double.Parse(split[6].Replace("円","").Replace(",",""));
                AllAssetAmount = double.Parse(split[3].Replace("円", "").Replace(",", ""));
            }
            catch (Exception e)
            {
                CashAmount = -1;
                AllAssetAmount = -1;
                return false;
            }
            return true;
        }


        /// <summary>
        /// 売却予定の銘柄があるかをチェック
        /// </summary>
        /// <returns></returns>
        private bool CheckSellDate()
        {
            var sql = "SELECT CodeNum,StockNum,Type FROM ContractTable WHERE State='Pos'";

            return true;
        }


        /// <summary>
        /// 本来は休日リストをDBに作るべきだが，今回はデータを追加した最終日を取得
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        private DateTime GetPrevWorkDay(DateTime today)
        {
            var sql = $"SELECT LastDate FROM GPrediction.dbo.Utils WHERE id = 0";
            var date = db.ExecuteScalar(sql, new DateTime(1900, 1, 1));
            if (date.Year != 1900)
                return date;
            else
            {
                var result = today;
                while (true)
                {
                    result = result.AddDays(-1);
                    if (result.DayOfWeek != DayOfWeek.Saturday && result.DayOfWeek != DayOfWeek.Sunday)
                        break;
                }
                return result;
            }
        }


        /// <summary>
        /// 今日市場が空いているかをチェックするメソッド
        /// </summary>
        /// <returns></returns>
        public bool IsOpeningToday()
        {
            var url = "https://finance.yahoo.co.jp/";
            var html = GetHTML(url);
            if (html.Contains("日本の証券市場はお休み"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// URLから該当のHTMLを返すメソッド
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetHTML(string url)
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Proxy = null;
                return wc.DownloadString(url);
            }
        }


        /// <summary>
        /// 市場開始前に行う処理を実行する 8:55くらい？
        /// </summary>
        public void OpeningAction()
        {
            //市場があく日かのチェック
            if (!IsOpeningToday())
                return;

            //今日売るべきものがあるかのチェック
            if (!CheckPortfolio())
                return;

            //DBをチェックして今日売るべき銘柄があるかをチェック
            if (!CheckSellDate())
                return;
        }


        /// <summary>
        /// ページが確実に切り替わるまで待機する
        /// </summary>
        private void Wait()
        {
            Application.DoEvents();
            while (webBrowser.IsBusy || webBrowser.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();
        }
    }
}
