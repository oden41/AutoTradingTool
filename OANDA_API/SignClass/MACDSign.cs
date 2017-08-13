using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    public class MACDSign
    {
        private List<double> closeStock;

        private List<double> ma1;
        private List<double> ma2;
        private List<double> MACD;

        List<double> signal;
        int dataNum;

        public MACDSign(List<double> closeStock)
        {
            signal = new List<double>();
            ma1 = new List<double>();
            ma2 = new List<double>();
            MACD = new List<double>();

            this.closeStock = closeStock;

            dataNum = closeStock.Count;

            CalcMACD();      
        }


        public Sign GetSign(int date)
        {
            ////売られすぎ買われすぎサインの判断
            if (signal[date - 1] > MACD[date - 1] && signal[date - 4] < MACD[date - 4])
            {
                //買われすぎ
                return Sign.TooBuy;
            }
            else if (signal[date - 1] < MACD[date - 1] && signal[date - 4] > MACD[date - 4])
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
            if (MACD[date - 1] < 0)
            {
                return Trend.Down;
            }
            else if (MACD[date - 1] > 0)
            {
                return Trend.Up;
            }
            else
            {
                return Trend.Steady;
            }
        }

        private void CalcMACD()
        {
            ma1.Clear();
            ma2.Clear();
            MACD.Clear();
            signal.Clear();

            int n1 = 5;
            int n2 = 20;


            for (int i = 0; i < dataNum; i++)
            {
                if (i < n1 - 1)
                {
                    ma1.Add(0);
                }
                else if (i == n1 - 1)
                {
                    ma1.Add(closeStock.GetRange(i - n1 + 1, n1).Average());
                }
                else
                {
                    ma1.Add(ma1[i - 1] + 2 * (closeStock[i] - ma1[i - 1]) / (n1 + 1));
                }

                if (i < n2 - 1)
                {
                    ma2.Add(0);
                    MACD.Add(0);
                    signal.Add(0);
                }
                else if (i == n2 - 1)
                {
                    ma2.Add(closeStock.GetRange(i - n2 + 1, n2).Average());
                    MACD.Add(ma1[i] - ma2[i]);
                    signal.Add(MACD.GetRange(i - 8, 9).Average());
                }
                else
                {
                    ma2.Add(ma2[i - 1] + 2 * (closeStock[i] - ma2[i - 1]) / (n2 + 1));
                    MACD.Add(ma1[i] - ma2[i]);
                    signal.Add(MACD.GetRange(i - 8, 9).Average());
                }
            }
        }
    }
}
