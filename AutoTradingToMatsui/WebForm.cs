using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTradingToMatsui
{
    public partial class WebForm : Form
    {
        const string loginURL = @"http://pocket.matsui.co.jp/member/servlet/Login?uid=NULLGWDOCOMO";
        const string mainURL = @"https://pocket.matsui.co.jp/member/servlet/Generic";
        private string ID { get { return ConfigurationManager.AppSettings["ID"]; } }//ログインID
        private string PASSWORD { get { return ConfigurationManager.AppSettings["PASSWORD"]; } }//ログインパスワード
        private string TRADEPASS { get { return ConfigurationManager.AppSettings["TRADEPASSWORD"]; } }//取引パス

        //銘柄コード -> [銘柄名,保有株数,取得平均,評価単価]
        private Dictionary<string, object[]> portfolio;

        public WebForm()
        {
            InitializeComponent();
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
                if (webBrowser.Url.AbsoluteUri != mainURL)
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
                if (webBrowser.Url.AbsoluteUri != mainURL)
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
                if (webBrowser.Url.AbsoluteUri != mainURL)
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
