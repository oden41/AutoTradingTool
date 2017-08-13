using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    public class DeviationSign
    {
        private List<double> closeStock;

        List<double> deviation;
        List<double> maLine;
        private double sigma;
        int dataNum;

        public DeviationSign(List<double> closeStock)
        {
            deviation = new List<double>();
            maLine = new List<double>();

            this.closeStock = closeStock;

            dataNum = closeStock.Count;

            CalcDeviation();
        }

        public Sign GetSign(int date)
        {
            ////売られすぎ買われすぎサインの判断
            if (deviation[date - 1] > sigma)
            {
                //買われすぎ
                return Sign.TooBuy;
            }
            else if (deviation[date - 1] < -sigma)
            {
                //売られすぎ
                return Sign.TooSell;
            }
            else
            {
                return Sign.Normal;
            }
        }

        private void CalcDeviation()
        {
            int n = 25;

            deviation.Clear();
            maLine.Clear();

            for (int i = 0; i < dataNum; i++)
            {
                if (i < n - 1)
                {
                    maLine.Add(0);
                }
                else
                {
                    maLine.Add(closeStock.GetRange(i - n + 1, n).Average());
                }

                if (i < n - 1)
                {
                    deviation.Add(0);
                }
                else
                {
                    deviation.Add((closeStock[i] - maLine[i]) * 100 / maLine[i]);
                }

            }

            sigma = deviation.Skip(n - 1).PopulationStandardDeviation();
        }
    }
}
