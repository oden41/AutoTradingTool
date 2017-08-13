using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    public class PsychologicalSign
    {
        private List<double> closeStock;
        List<double> psychological;
        int dataNum;

        public PsychologicalSign(List<double> closeStock)
        {
            psychological = new List<double>();
            this.closeStock = closeStock;

            dataNum = closeStock.Count;

            CalcROC();
            //PrintChart();       
        }

        public Sign GetSign(int date)
        {
            ////売られすぎ買われすぎサインの判断
            if (psychological[date - 1] > 75)
            {
                //買われすぎ
                return Sign.TooBuy;
            }
            else if (psychological[date - 1] < 25)
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
            psychological.Clear();
            for (int i = 0; i < dataNum; i++)
            {
                if (i < 12)
                {
                    psychological.Add(0);
                }
                else
                {
                    double upCount = 0;
                    for (int j = 0; j < 12; j++)
                    {
                        if (closeStock[i - j - 1] < closeStock[i - j])
                            upCount++;
                        else if (closeStock[i - j - 1] == closeStock[i - j])
                            upCount += 0.5;
                    }

                    psychological.Add(upCount * 100 / 12);
                }
            }
        }
    }
}
