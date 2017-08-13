using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTool
{
    public enum Sign
    {
        TooBuy = -1,
        Normal,
        TooSell
    }

    public static class SignString
    {
        // 拡張メソッド
        public static string GetString(this Sign value)
        {
            string[] values = { "Sell", "Normal", "Buy" };
            return values[(int)value + 1];
        }
    }
}
