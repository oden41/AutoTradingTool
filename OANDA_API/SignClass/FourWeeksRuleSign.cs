using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    public class FourWeeksRuleSign
    {
        private List<double> closeStock;
        int dataNum;

        double high20;
        double high5;
        double low5;
        double low20;

        public FourWeeksRuleSign(List<double> highStock, List<double> lowStock, List<double> closeStock)
        {
            //各種線の準備
            this.closeStock = closeStock;

            dataNum = closeStock.Count;

            high20 = highStock.GetRange(dataNum - 21, 20).Max();
            high5 = highStock.GetRange(dataNum - 6, 5).Max();
            low5 = lowStock.GetRange(dataNum - 6, 5).Min();
            low20 = lowStock.GetRange(dataNum - 21, 20).Min();
        }


        /// <summary>
        /// 2日ルール撤廃
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public Sign GetSign(int date)
        {
            //買われすぎ
            if (closeStock[date - 1] < low20 && closeStock[date - 2] > low20)
            {
                return Sign.TooBuy;
            }
            //売られすぎ
            else if (closeStock[date - 1] > high20 && closeStock[date - 2] < high20)
            {
                return Sign.TooSell;
            }
            else
            {
                return Sign.Normal;
            }
        }
    }
}
