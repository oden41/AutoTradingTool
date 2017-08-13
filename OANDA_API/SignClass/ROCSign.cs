using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    public class ROCSign
    {
        private List<double> closeStock;
        List<double> ROC;
        private double sigma;
        int dataNum;

        public ROCSign(List<double> closeStock)
        {
            ROC = new List<double>();
            this.closeStock = closeStock;

            dataNum = closeStock.Count;

            CalcROC();
        }

        public Sign GetSign(int date)
        {
            ////売られすぎ買われすぎサインの判断
            if (ROC[date - 1] > sigma ||
                (ROC[date - 1] < 0 && ROC[date - 3] > 0))
            {
                //買われすぎ
                return Sign.TooBuy;
            }
            else if (ROC[date - 1] < -sigma * 1.5 ||
                (ROC[date - 1] > 0 && ROC[date - 3] < 0))
            {
                //売られすぎ
                return Sign.TooSell;
            }
            else
            {
                return Sign.Normal;
            }
        }

        private void CalcROC()
        {
            int n = 10;

            ROC.Clear();
            for (int i = 0; i < dataNum; i++)
            {
                if (i < n)
                {
                    ROC.Add(0);
                }
                else
                {
                    double roc = (closeStock[i] - closeStock[i - n]) * 100 / closeStock[i - n];
                    ROC.Add(roc);
                }
            }

            sigma = ROC.Skip(n).PopulationStandardDeviation();
        }
    }
}
