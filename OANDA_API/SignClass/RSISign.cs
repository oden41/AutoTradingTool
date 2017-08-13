using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    public class RSISign
    {
        private List<double> closeStock;

        private List<double> ma5;
        private List<double> ma25;

        List<double> RSI;
        private double sigma;
        private double average;
        int dataNum;

        public RSISign(List<double> closeStock)
        {
            RSI = new List<double>();
            ma5 = new List<double>();
            ma25 = new List<double>();

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
            }

            CalcRSI();
        }

        public Sign GetSign(int date)
        {
            ////売られすぎ買われすぎサインの判断
            if (RSI[date - 1] > 70)
            {
                //買われすぎ
                return Sign.TooBuy;
            }
            else if (RSI[date - 1] < 25)
            {
                //売られすぎ
                return Sign.TooSell;
            }
            else
            {
                return Sign.Normal;
            }
        }

        private void CalcRSI()
        {

            RSI.Clear();

            for (int i = 0; i < dataNum; i++)
            {
                if (i < 14)
                {
                    RSI.Add(0);
                }
                else
                {
                    double upVal = 0;
                    double downVal = 0;
                    for (int j = 0; j < 14; j++)
                    {
                        if (closeStock[i - j - 1] < closeStock[i - j])
                            upVal += closeStock[i - j] - closeStock[i - j - 1];
                        else
                            downVal += closeStock[i - j - 1] - closeStock[i - j];
                    }

                    RSI.Add(upVal * 100 / (upVal + downVal));
                }
            }

            average = RSI.Skip(13).Average();
            sigma = RSI.Skip(13).PopulationStandardDeviation();
        }
    }
}
