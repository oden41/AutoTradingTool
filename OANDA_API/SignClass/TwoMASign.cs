using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    public class TwoMASign
    {
        private List<double> maLong;
        private List<double> maShort;
        private List<double> closeStock;
        int dataNum;
        int termShort;
        int termLong;

        public TwoMASign(List<double> closeStock)
        {
            maLong = new List<double>();
            maShort = new List<double>();

            //各種線の準備
            this.closeStock = closeStock;
            dataNum = closeStock.Count;

            CalcMA();
        }

        private void CalcMA()
        {
            maLong.Clear();
            maShort.Clear();
            termShort = 10;
            termLong = 40;            

            for (int i = 0; i < closeStock.Count; i++)
            {
                if (i < termShort - 1)
                {
                    maShort.Add(0);
                }
                else
                {
                    maShort.Add(closeStock.GetRange(i - termShort + 1, termShort).Average());
                }

                if (i < termLong - 1)
                {
                    maLong.Add(0);
                }
                else
                {
                    maLong.Add(closeStock.GetRange(i - termLong + 1, termLong).Average());
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
            if (maShort[date - 1] < maLong[date - 1] && maShort[date - 2] < maLong[date - 2] && maShort[date - 3] > maLong[date - 3] && maShort[date - 4] > maLong[date - 4])
            {
                //買われすぎ
                return Sign.TooBuy;
            }
            else if (maLong[date - 1] < maShort[date - 1] && maLong[date - 2] < maShort[date - 2] && maLong[date - 3] > maShort[date - 3] && maLong[date - 4] > maShort[date - 4])
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
