using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    public class StochasticSign
    {
        private List<double> highStock;
        private List<double> lowStock;
        private List<double> closeStock;
        private List<double> nlow;
        private List<double> nhigh;

        List<double> K;
        List<double> D;
        List<double> SlowD;
        int dataNum;

        public StochasticSign(List<double> highStock, List<double> lowStock, List<double> closeStock)
        {
            K = new List<double>();
            D = new List<double>();
            SlowD = new List<double>();

            nlow = new List<double>();
            nhigh = new List<double>();

            this.highStock = highStock;
            this.lowStock = lowStock;
            this.closeStock = closeStock;

            dataNum = closeStock.Count;

            CalcStochastic();
        }

        public Sign GetSign(int date)
        {
            ////売られすぎ買われすぎサインの判断
            if (D[date - 1] > 80 && SlowD[date - 1] > 80 && (SlowD[date - 1] > D[date - 1] && SlowD[date - 4] < D[date - 4]) ||
                D[date - 1] > 80 && K[date - 1] > 80 && (K[date - 1] < D[date - 1] && K[date - 4] > D[date - 4]))
            {
                //買われすぎ
                return Sign.TooBuy;
            }
            else if (D[date - 1] < 20 && SlowD[date - 1] < 20 && (SlowD[date - 1] < D[date - 1] && SlowD[date - 4] > D[date - 4]) ||
                D[date - 1] < 20 && K[date - 1] < 20 && (K[date - 1] > D[date - 1] && K[date - 4] < D[date - 4]))
            {
                //売られすぎ
                return Sign.TooSell;
            }
            else
            {
                return Sign.Normal;
            }
        }

        /// <summary>
        /// nhigh = close - low
        /// nlow = high - low
        /// </summary>
        private void CalcStochastic()
        {
            int n = 14;

            D.Clear();
            K.Clear();
            SlowD.Clear();
            nhigh.Clear();
            nlow.Clear();

            for (int i = 0; i < dataNum; i++)
            {
                if (i < n - 1)
                {
                    K.Add(0);
                    nhigh.Add(0);
                    nlow.Add(0);
                }
                else
                {
                    double max, min;
                    max = highStock.GetRange(i - n + 1, n).Max();
                    min = lowStock.GetRange(i - n + 1, n).Min();
                    nhigh.Add(closeStock[i] - min);
                    nlow.Add(max - min);

                    K.Add((closeStock[i] - min) * 100 / (max - min));
                }
            }

            for (int i = 0; i < dataNum; i++)
            {
                if (i < n + 1)
                {
                    D.Add(0);
                }
                else
                {
                    double bunsi = nhigh.GetRange(i - 2, 3).Sum();
                    double bunbo = nlow.GetRange(i - 2, 3).Sum();

                    D.Add(bunsi * 100 / bunbo);
                }
            }

            for (int i = 0; i < dataNum; i++)
            {
                if (i < n + 3)
                {
                    SlowD.Add(0);
                }
                else
                {
                    SlowD.Add(D.GetRange(i - 2, 3).Average());
                }
            }
        }
    }
}
