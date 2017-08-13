using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    struct ParaPoint
    {
        public double data;

        public ParaPoint(double data)
        {
            this.data = data;
        }
    }

    public class ParabolicSign
    {
        private List<double> highStock;
        private List<double> lowStock;
        private List<double> closeStock;
        private List<ParaPoint> parabolic;
        int dataNum;
        double sar, ep, af;

        public Sign BuySign
        {
            get;
            set;
        }

        public ParabolicSign(List<double> highStock, List<double> lowStock,  List<double> closeStock)
        {
            parabolic = new List<ParaPoint>();

            //各種線の準備
            this.highStock = highStock;
            this.lowStock = lowStock;
            this.closeStock = closeStock;


            dataNum = closeStock.Count;
         
            CalcSAR(0.02);
        }

        public Sign GetSign(int date)
        {
            //売られすぎ買われすぎサインの判断
            if (closeStock[date - 1] < parabolic[date - 1].data && closeStock[date - 2] > parabolic[date - 2].data)
            {
                //売りサイン
                return Sign.TooBuy;
            }
            else if (closeStock[date - 1] > parabolic[date - 1].data && closeStock[date - 2] < parabolic[date - 2].data)
            {
                //買いサイン
                return Sign.TooSell;
            }
            else
            {
                return Sign.Normal;
            }
        }

        public Trend GetTrend(int date)
        {
            //Trendの判断
            if (closeStock[date - 1] < parabolic[date - 1].data)
            {
                return Trend.Down;
            }
            else if (closeStock[date - 1] > parabolic[date - 1].data)
            {
                return Trend.Up;
            }
            else
            {
                return Trend.Steady;
            }
        }

        private void CalcSAR(double af)
        {
            parabolic.Clear();

            //5日間で上昇トレンドか下降トレンドかを決定する
            for (int i = 0; i < 5; i++)
            {

                parabolic.Add(new ParaPoint(0));
            }

            if (closeStock[0] < closeStock[4])
            {
                BuySign = Sign.TooSell;
                sar = lowStock.GetRange(0, 5).Min();
                ep = highStock.GetRange(0, 5).Max();
            }
            else
            {
                BuySign = Sign.TooBuy;
                sar = highStock.GetRange(0, 5).Max();
                ep = lowStock.GetRange(0, 5).Min();
            }

            this.af = af;

            ParaPoint point = new ParaPoint(0);
            for (int i = 5; i < dataNum; i++)
            {
                if (BuySign == Sign.TooSell && ep < highStock[i])
                {
                    ep = highStock[i];
                    this.af = Math.Min(0.2, this.af + af);
                }
                else if (BuySign == Sign.TooBuy && ep > lowStock[i])
                {
                    ep = lowStock[i];
                    this.af += Math.Min(0.2, this.af + af);
                }

                sar = (ep - sar) * this.af + sar;

                if ((BuySign == Sign.TooSell && sar > lowStock[i]) ||
                    (BuySign == Sign.TooBuy && sar < highStock[i]))
                {

                    if (BuySign == Sign.TooSell)
                    {
                        BuySign = Sign.TooBuy;
                        sar = highStock.GetRange(i - 4, 5).Max();
                        ep = lowStock.GetRange(i - 4, 5).Min();
                        this.af = af;
                    }
                    else if (BuySign == Sign.TooBuy)
                    {
                        BuySign = Sign.TooSell;
                        sar = lowStock.GetRange(i - 4, 5).Min();
                        ep = highStock.GetRange(i - 4, 5).Max();
                        this.af = af;
                    }

                    point.data = sar;
                    parabolic.Add(point);
                }
                else
                {
                    point.data = sar;
                    parabolic.Add(point);
                }
            }
        }
    }
}
