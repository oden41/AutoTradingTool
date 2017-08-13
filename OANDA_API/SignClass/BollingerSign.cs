using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    public class BollingerSign
    {
        private List<double> closeStock;
        List<double> ma25;
        List<double> sigma;
        int dataNum;

        public BollingerSign(List<double> closeStock)
        {
            ma25 = new List<double>();
            sigma = new List<double>();
            this.closeStock = closeStock;

            dataNum = closeStock.Count;

            for (int i = 0; i < closeStock.Count; i++)
            {
                //if (i < 4)
                //{
                //    ma5.Add(0);
                //}
                //else
                //{
                //    ma5.Add(closeVal.GetRange(i - 4, 5).Average());
                //}

                if (i < 24)
                {
                    ma25.Add(0);
                }
                else
                {
                    ma25.Add(closeStock.GetRange(i - 24, 25).Average());
                }

                //if (i < 74)
                //{
                //    ma75.Add(0);
                //}
                //else
                //{
                //    ma75.Add(closeVal.GetRange(i - 74, 75).Average());
                //}
            }

            //sigma計算
            for (int i = 0; i < ma25.Count; i++)
            {
                if (ma25[i] == 0)
                {
                    sigma.Add(0);
                }
                else
                {
                    double value = 0;
                    for (int j = 0; j < 25; j++)
                    {
                        value += Math.Pow(closeStock[i - j] - ma25[i], 2);
                    }

                    double sigmaVal = Math.Sqrt(value / (25 - 1));
                    sigma.Add(sigmaVal);
                }
            }
        }

        public Sign GetSign(int date)
        {
            //売られすぎ買われすぎサインの判断
            if (closeStock[date - 1] > ma25[date - 1] + sigma[date - 1] * 2)
            {
                //買われすぎ
                return Sign.TooBuy;
            }
            else if (closeStock[date - 1] < ma25[date - 1] - sigma[date - 1] * 2)
            {
                //売られすぎ
                return Sign.TooSell;
            }
            else
            {
                return Sign.Normal;
            }
        }

    }
}
