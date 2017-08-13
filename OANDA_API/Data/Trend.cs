using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTool
{
    public enum Trend
    {
        Down = -1,
        Steady,
        Up
    }
    public static class TrendString
    {
        // 拡張メソッド
        public static string GetString(this Trend value)
        {
            string[] values = { "Down", "Steady", "Up" };
            return values[(int)value + 1];
        }
    }
}
