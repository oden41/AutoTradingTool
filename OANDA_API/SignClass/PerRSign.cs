using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    public class PerRSign
    {
        private List<double> highStock;
        private List<double> lowStock;
        private List<double> closeStock;

        List<double> perR;
        int dataNum;

        public PerRSign(List<double> highStock, List<double> lowStock, List<double> closeStock)
        {
            perR = new List<double>();

            this.highStock = highStock;
            this.lowStock = lowStock;
            this.closeStock = closeStock;

            dataNum = closeStock.Count;

            CalcPerR();
            //PrintChart();
        }

        public Sign GetSign(int date)
        {
            ////売られすぎ買われすぎサインの判断
            if (perR[date - 5] < 30 && perR[date - 1] > 50 ||
                perR[date - 1] < 10)
            {
                //買われすぎ
                return Sign.TooBuy;
            }
            else if (perR[date - 5] > 70 && perR[date - 1] < 50 ||
                perR[date - 1] > 90)
            {
                //売られすぎ
                return Sign.TooSell;
            }
            else
            {
                return Sign.Normal;
            }
        }

        private void CalcPerR()
        {
            perR.Clear();

            for (int i = 0; i < dataNum; i++)
            {
                if (i < 19)
                {
                    perR.Add(0);
                }
                else
                {
                    double max, min;
                    max = highStock.GetRange(i - 19, 20).Max();
                    min = lowStock.GetRange(i - 19, 20).Min();

                    perR.Add((max - closeStock[i]) * 100 / (max - min));
                }
            }
        }
    }
}
