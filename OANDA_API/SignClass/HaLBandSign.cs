using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    public class HaLBandSign
    {
        private List<double> maHigh;
        private List<double> maLow;
        private List<double> closeStock;
        int dataNum;
        int term;

        public HaLBandSign(List<double> highStock, List<double> lowStock, List<double> closeStock)
        {
            maHigh = new List<double>();
            maLow = new List<double>();

            this.closeStock = closeStock;
            dataNum = closeStock.Count;

            CalcMA(highStock,lowStock);
        }

        private void CalcMA(List<double> highStock, List<double> lowStock)
        {
            maHigh.Clear();
            maLow.Clear();
            term = 20;

            for (int i = 0; i < closeStock.Count; i++)
            {
                if (i < term - 1)
                {
                    maHigh.Add(0);
                    maLow.Add(0);
                }
                else
                {
                    maHigh.Add(highStock.GetRange(i - term + 1, term).Average());
                    maLow.Add(lowStock.GetRange(i - term + 1, term).Average());
                }
            }
        }


        /// <summary>
        /// 売買サインを出す　2日ルールを適用している
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public Sign GetSign(int date)
        {
            //売られすぎ買われすぎサインの判断
            if (maLow[date - 1] > closeStock[date - 1] && maLow[date - 2] > closeStock[date - 2] && maLow[date - 3] < closeStock[date - 3] && maLow[date - 4] < closeStock[date - 4])
            {
                //買われすぎ
                return Sign.TooBuy;
            }
            else if (maHigh[date - 1] < closeStock[date - 1] && maHigh[date - 2] < closeStock[date - 2] && maHigh[date - 3] > closeStock[date - 3] && maHigh[date - 4] > closeStock[date - 4])
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
