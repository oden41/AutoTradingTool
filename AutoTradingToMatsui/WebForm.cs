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
        const string url = @"http://pocket.matsui.co.jp/member/servlet/Login?uid=NULLGWDOCOMO";
        private string ID { get { return ConfigurationManager.AppSettings["ID"]; } }//ログインID
        private string PASSWORD { get { return ConfigurationManager.AppSettings["PASSWORD"]; } }//ログインパスワード
        private string TRADEPASS { get { return ConfigurationManager.AppSettings["TRADEPASSWORD"]; } }//取引パス

        public WebForm()
        {
            InitializeComponent();
            webBrowser.NavigateAndWait(url);
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
            var htmlCollection = webBrowser.Document.GetElementsByTagName("input");
            htmlCollection.GetElementsByName("clientCD")[0].InnerText = ID;
            htmlCollection.GetElementsByName("passwd")[0].InnerText = PASSWORD;
            htmlCollection[8].InvokeMember("click"); // フォームのサブミット
            Wait();
        }

        /// <summary>
        /// メインページの状態から現物買の注文まで行う
        /// </summary>
        /// <param name="code"></param>
        /// <param name="stockUnit"></param>
        /// <param name="price"></param>
        /// <param name="isNariyuki"></param>
        /// <param name="execCond"></param>
        /// <param name="validDT"></param>
        /// <returns></returns>
        public bool Order(string code, int stockUnit, double price, bool isNariyuki, int execCond = 0, int validDT = 0)
        {
            try
            {
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
                if (webBrowser.DocumentText.Contains("時間外エラー"))
                    throw new Exception("時間外エラー");
            }
            catch (Exception e)
            {                
                return false;
            }

            return true;
        }

        private void Wait()
        {
            Application.DoEvents();
            while (webBrowser.IsBusy || webBrowser.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();
        }
    }
}
