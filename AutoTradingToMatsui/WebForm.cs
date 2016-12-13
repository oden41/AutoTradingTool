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
        }
    }
}
