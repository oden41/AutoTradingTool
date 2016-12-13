using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTradingToMatsui
{
    /// <summary>
    /// WebBrowserで指定したURLが表示されるまで待機するよう拡張
    /// </summary>
    class ExWebBrowser : WebBrowser
    {

        bool done;

        // タイムアウト時間（10秒）
        TimeSpan timeout = new TimeSpan(0, 0, 10);

        protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        {
            // ページにフレームが含まれる場合にはフレームごとに
            // このメソッドが実行されるため実際のURLを確認する
            if (e.Url == Url)
            {
                done = true;
            }
        }

        public ExWebBrowser()
        {
            // スクリプト・エラーを表示しない
            this.ScriptErrorsSuppressed = true;
        }

        public bool NavigateAndWait(string url)
        {

            Navigate(url); // ページの移動

            done = false;
            DateTime start = DateTime.Now;

            while (done == false)
            {
                if (DateTime.Now - start > timeout)
                {
                    // タイムアウト
                    return false;
                }
                Application.DoEvents();
            }
            return true;
        }
    }
}
