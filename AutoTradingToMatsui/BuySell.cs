using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTradingToMatsui
{
    enum BuySell
    {
        Buy,
        Sell
    }

    // enum定義のヘルパクラス
    static class BSExt
    {
        // Gender に対する拡張メソッドの定義
        public static string DisplayName(this BuySell bs)
        {
            string[] names = { "Buy", "Sell" };
            return names[(int)bs];
        }
    }
}
