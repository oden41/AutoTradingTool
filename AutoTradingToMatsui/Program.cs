using AutoTradingToMatsui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTradingAtMatsui
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Application.Run(new WebForm());
            var form = new WebForm();
            form.Login();
            //form.BuyOrder("9739", 100, 1000, false);
            form.CheckPortfolio();
            form.ShowDialog();
        }
    }
}
