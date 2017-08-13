using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    public class MASign
    {
        private List<double> ma5;
        private List<double> ma25;
        private List<double> ma75;
        private List<double> closeStock;
        int dataNum;

        public MASign(List<double> closeStock)
        {
            ma5 = new List<double>();
            ma25 = new List<double>();
            ma75 = new List<double>();

            this.closeStock = closeStock;
            dataNum = closeStock.Count;

            for (int i = 0; i < closeStock.Count; i++)
            {
                if (i < 4)
                {
                    ma5.Add(0);
                }
                else
                {
                    ma5.Add(closeStock.GetRange(i - 4, 5).Average());
                }

                if (i < 24)
                {
                    ma25.Add(0);
                }
                else
                {
                    ma25.Add(closeStock.GetRange(i - 24, 25).Average());
                }

                if (i < 74)
                {
                    ma75.Add(0);
                }
                else
                {
                    ma75.Add(closeStock.GetRange(i - 74, 75).Average());
                }
            }
        }

        public Sign GetSign(int date)
        {
            //売られすぎ買われすぎサインの判断
            if (ma25[date - 1] > ma5[date - 1] && ma25[date - 4] < ma5[date - 4])
            {
                //買われすぎ
                return Sign.TooBuy;
            }
            else if (ma25[date - 1] < ma5[date - 1] && ma25[date - 4] > ma5[date - 4])
            {
                //売られすぎ
                return Sign.TooSell;
            }
            else
            {
                return Sign.Normal;
            }
        }

        public Trend GetTrend(int date)
        {
            //Trendの判断
            if (closeStock[date - 1] < ma5[date - 1] && ma5[date - 1] < ma25[date - 1])
            {
                return Trend.Down;
            }
            else if (closeStock[date - 1] > ma5[date - 1] && ma5[date - 1] > ma25[date - 1])
            {
                return Trend.Up;
            }
            else
            {
                return Trend.Steady;
            }
        }
    }
}
