using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    public class DMISign
    {
        private List<double> highStock;
        private List<double> lowStock;
        private List<double> closeStock;

        List<double> DMI;
        List<double> TR;
        List<double> pDM;
        List<double> sDM;
        List<double> pDI;
        List<double> sDI;
        List<double> DX;
        List<double> ADX;
        int dataNum;

        public DMISign(List<double> highStock, List<double> lowStock, List<double> closeStock)
        {
            DMI = new List<double>();
            TR = new List<double>();
            pDM = new List<double>();
            sDM = new List<double>();
            pDI = new List<double>();
            sDI = new List<double>();
            DX = new List<double>();
            ADX = new List<double>();

            this.highStock = highStock;
            this.lowStock = lowStock;
            this.closeStock = closeStock;
            
            dataNum = closeStock.Count;

            CalcDMI();
        }


        public Sign GetSign(int date)
        {
            ////売られすぎ買われすぎサインの判断
            if (pDI[date - 1] < sDI[date - 1] && pDI[date - 3] > sDI[date - 3] /*&& ADX[date - 1] > sDI[date - 1] && ADX[date - 3] < sDI[date - 3]*/)
            {
                //買われすぎ
                return Sign.TooBuy;
            }
            else if (pDI[date - 1] > sDI[date - 1] && pDI[date - 3] < sDI[date - 3] /*&& ADX[date - 1] > sDI[date - 1] && ADX[date - 3] < sDI[date - 3]*/)
            {
                //売られすぎ
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
            if (pDI[date - 1] - sDI[date - 1] < -7.5)
            {
                return Trend.Down;
            }
            else if (pDI[date - 1] - sDI[date - 1] > 7.5)
            {
                return Trend.Up;
            }
            else
            {
                return Trend.Steady;
            }
        }

        private void CalcDMI()
        {

            DMI.Clear();
            TR.Clear();
            pDM.Clear();
            sDM.Clear();
            pDI.Clear();
            sDI.Clear();
            DX.Clear();
            ADX.Clear();

            for (int i = 0; i < dataNum; i++)
            {
                if (i < 1)
                {
                    pDM.Add(0);
                    sDM.Add(0);
                    TR.Add(0);
                }
                else
                {
                    if (highStock[i] < highStock[i - 1] && lowStock[i - 1] < lowStock[i])
                    {
                        pDM.Add(0);
                        sDM.Add(0);
                    }
                    else if ((highStock[i] - highStock[i - 1]) > (lowStock[i - 1] - lowStock[i]))
                    {
                        pDM.Add(highStock[i] - highStock[i - 1]);
                        sDM.Add(0);
                    }
                    else if ((highStock[i] - highStock[i - 1]) < (lowStock[i - 1] - lowStock[i]))
                    {
                        pDM.Add(0);
                        sDM.Add(lowStock[i - 1] - lowStock[i]);
                    }
                    else
                    {
                        pDM.Add(0);
                        sDM.Add(0);
                    }

                    TR.Add(Math.Max(Math.Max(highStock[i] - closeStock[i - 1], closeStock[i - 1] - lowStock[i]), highStock[i] - lowStock[i]));
                }

                if (i < 14)
                {
                    pDI.Add(0);
                    sDI.Add(0);
                    DX.Add(0);
                }
                else
                {
                    double rtr = TR.GetRange(i - 13, 14).Sum();
                    pDI.Add(pDM.GetRange(i - 13, 14).Sum() * 100 / rtr);
                    sDI.Add(sDM.GetRange(i - 13, 14).Sum() * 100 / rtr);
                    DX.Add(Math.Abs(pDI[i] - sDI[i]) * 100 / (pDI[i] + sDI[i]));
                }

                if (i < 14 + 14 - 1)
                {
                    ADX.Add(0);
                }
                else
                {
                    ADX.Add(DX.GetRange(i - 13, 14).Average());
                }
            }
        }
    }
}
