using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    struct RCIData
    {
        public double close;
        public int rankA;
        public int rankB;

        public RCIData(double close, int rankA, int rankB)
        {
            this.close = close;
            this.rankA = rankA;
            this.rankB = rankB;
        }
    }

    public class RCISign
    {
        private List<double> closeStock;

        List<double> RCI5;
        List<double> RCI10;
        int dataNum;

        public RCISign(List<double> closeStock)
        {
            RCI5 = new List<double>();
            RCI10 = new List<double>();
            this.closeStock = closeStock;

            dataNum = closeStock.Count;

            CalcRCI();
        }

        public Sign GetSign(int date)
        {
            ////売られすぎ買われすぎサインの判断
            if (RCI5[date - 1] >= 85)
            {
                //買われすぎ
                return Sign.TooBuy;
            }
            else if (RCI5[date - 1] > -85 && (RCI5[date - 4] <= -85 || RCI5[date - 6] <= -85))
            {
                //3日or5日前が-85％以下で，今日が85％以上
                //売られすぎ
                return Sign.TooSell;
            }
            else
            {
                return Sign.Normal;
            }
        }

        private void CalcRCI()
        {

            RCI5.Clear();
            RCI10.Clear();

            for (int i = 0; i < dataNum; i++)
            {
                if (i < 4)
                {
                    RCI5.Add(0);
                }
                else
                {
                    RCIData[] data = new RCIData[5];

                    for (int j = 0; j < 5; j++)
                    {
                        data[j] = new RCIData(closeStock[i - j], j + 1, 0);
                    }

                    Array.Sort(data, (x, y) => (int)((y.close - x.close) * 100));

                    int sumrank = 0;
                    for (int j = 0; j < 5; j++)
                    {
                        data[j].rankB = j + 1;
                        sumrank += (int)Math.Pow((data[j].rankA - data[j].rankB), 2);
                    }

                    RCI5.Add((1 - 6 * (double)sumrank / (5 * (5 * 5 - 1))) * 100);
                }

                if (i < 9)
                {
                    RCI10.Add(0);
                }
                else
                {
                    RCIData[] data = new RCIData[10];

                    for (int j = 0; j < 10; j++)
                    {
                        data[j] = new RCIData(closeStock[i - j], j + 1, 0);
                    }

                    Array.Sort(data, (x, y) => (int)((y.close - x.close) * 100));

                    int sumrank = 0;
                    for (int j = 0; j < 10; j++)
                    {
                        data[j].rankB = j + 1;
                        sumrank += (int)Math.Pow((data[j].rankA - data[j].rankB), 2);
                    }

                    RCI10.Add((1 - 6 * (double)sumrank / (10 * (10 * 10 - 1))) * 100);
                }
            }
        }
    }
}
