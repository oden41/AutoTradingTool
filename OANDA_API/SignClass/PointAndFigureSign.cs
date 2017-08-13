using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    public class PointAndFigureSign
    {
        private List<double> closeStock;
        private List<PointAndFigureData> pfData;
        int dataNum;
        int reversePoint;
        double oneBoxNorm;

        public PointAndFigureSign(List<double> closeStock)
        {
            pfData = new List<PointAndFigureData>();

            //各種線の準備
            this.closeStock = closeStock;
            dataNum = closeStock.Count;

            CalcPF();
        }


        /// <summary>
        /// SIMPLE BULLISH BUY SIGNAL
        /// 1つ前の高値を更新 終値2日ルール撤廃
        /// </summary>
        /// <returns></returns>
        public bool isB1Sign()
        {
            try
            {
                if (pfData[pfData.Count - 3].BuyorSell != Sign.TooSell)
                    return false;

                double lastHighValue = pfData[pfData.Count - 3].Data.Value;
                if (lastHighValue > closeStock[dataNum - 2] && lastHighValue < closeStock[dataNum - 1])
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// SIMPLE BULLISH BUY SIGNAL WITH A RISING BOTTOM
        /// 底値を切り上げながら1つ前の高値を更新 終値2日ルール撤廃
        /// </summary>
        /// <returns></returns>
        public bool isB2Sign()
        {
            try
            {
                if (pfData[pfData.Count - 3].BuyorSell != Sign.TooSell)
                    return false;

                if (pfData[pfData.Count - 2].Data.Value <= pfData[pfData.Count - 4].Data.Value)
                    return false;

                double lastHighValue = pfData[pfData.Count - 3].Data.Value;
                if (lastHighValue > closeStock[dataNum - 2] && lastHighValue < closeStock[dataNum - 1])
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// BREAKOUT OF A TRIPLE TOP
        /// 前2列の上昇をさらにブレイク　終値2日ルール撤廃
        /// </summary>
        /// <returns></returns>
        public bool isB3Sign()
        {
            try
            {
                if (pfData[pfData.Count - 3].BuyorSell != Sign.TooSell || pfData[pfData.Count - 5].BuyorSell != Sign.TooSell)
                    return false;

                double lastHighValue = pfData[pfData.Count - 3].Data.Value;
                double lastHighValue2 = pfData[pfData.Count - 5].Data.Value;
                if ((lastHighValue > closeStock[dataNum - 2] || lastHighValue2 > closeStock[dataNum - 2]) &&
                    (lastHighValue < closeStock[dataNum - 1] && lastHighValue2 < closeStock[dataNum - 1]))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// ASCENDING TRIPLE TOP
        /// 抵抗線，支持線ともに切り上げながらのブレイク 終値2日ルール撤廃
        /// </summary>
        /// <returns></returns>
        public bool isB4Sign()
        {
            try
            {
                if (pfData[pfData.Count - 3].BuyorSell != Sign.TooSell || pfData[pfData.Count - 5].BuyorSell != Sign.TooSell)
                    return false;

                if (pfData[pfData.Count - 3].Data.Value <= pfData[pfData.Count - 5].Data.Value)
                    return false;

                if (pfData[pfData.Count - 2].Data.Value <= pfData[pfData.Count - 4].Data.Value)
                    return false;

                double lastHighValue = pfData[pfData.Count - 3].Data.Value;
                if (lastHighValue > closeStock[dataNum - 2] && lastHighValue < closeStock[dataNum - 1])
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// SPREAD TRIPLE TOP
        /// 前3列の上昇をブレイク　終値2日ルール撤廃
        /// </summary>
        /// <returns></returns>
        public bool isB5Sign()
        {
            try
            {
                if (pfData[pfData.Count - 3].BuyorSell != Sign.TooSell || pfData[pfData.Count - 5].BuyorSell != Sign.TooSell || pfData[pfData.Count - 7].BuyorSell != Sign.TooSell)
                    return false;

                double lastHighValue = pfData[pfData.Count - 3].Data.Value;
                double lastHighValue2 = pfData[pfData.Count - 5].Data.Value;
                double lastHighValue3 = pfData[pfData.Count - 7].Data.Value;
                if ((lastHighValue > closeStock[dataNum - 2] || lastHighValue2 > closeStock[dataNum - 2] || lastHighValue3 > closeStock[dataNum - 2]) &&
                    (lastHighValue < closeStock[dataNum - 1] && lastHighValue2 < closeStock[dataNum - 1] && lastHighValue3 < closeStock[dataNum - 1]))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// SIMPLE SELL SIGNAL
        /// 前回の安値を更新
        /// </summary>
        /// <returns></returns>
        public bool isS1Sign()
        {
            try
            {
                if (pfData[pfData.Count - 3].BuyorSell != Sign.TooBuy)
                    return false;

                double lastLowValue = pfData[pfData.Count - 3].Data.Value;
                if (lastLowValue < closeStock[dataNum - 2] && lastLowValue > closeStock[dataNum - 1])
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// SIMPLE SELL SIGNAL WITH A DECLINING TOP
        /// 上値を切り下げながら前回の安値を更新
        /// </summary>
        /// <returns></returns>
        public bool isS2Sign()
        {
            try
            {
                if (pfData[pfData.Count - 3].BuyorSell != Sign.TooBuy)
                    return false;

                if (pfData[pfData.Count - 2].Data.Value >= pfData[pfData.Count - 4].Data.Value)
                    return false;

                double lastLowValue = pfData[pfData.Count - 3].Data.Value;
                if (lastLowValue < closeStock[dataNum - 2] && lastLowValue > closeStock[dataNum - 1])
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// ポイント・アンド・フィギュアの作成
        /// </summary>
        private void CalcPF()
        {
            reversePoint = 3;
            oneBoxNorm = getOnePoint();            

            pfData.Clear();

            bool isFirst = true;
            for (int i = 0; i < closeStock.Count(); i++)
            {
                if (isFirst)
                {
                    if (closeStock[0] < closeStock[i] && floor(closeStock[0], oneBoxNorm) + oneBoxNorm < floor(closeStock[i], oneBoxNorm) ||
                        closeStock[0] > closeStock[i] && ceil(closeStock[0], oneBoxNorm) - oneBoxNorm > ceil(closeStock[i], oneBoxNorm))
                    {
                        PointAndFigureData data = new PointAndFigureData();
                        data.Data = new KeyValuePair<double, double>(closeStock[0] < closeStock[i] ? ceil(closeStock[0], oneBoxNorm) : floor(closeStock[0], oneBoxNorm), (closeStock[0] < closeStock[i] ? floor(closeStock[i], oneBoxNorm) : ceil(closeStock[i], oneBoxNorm)));
                        pfData.Add(data);
                        isFirst = false;
                    }
                }
                else
                {
                    if (pfData[pfData.Count - 1].BuyorSell == Sign.TooBuy)
                    {
                        //更に下がる場合
                        if (pfData[pfData.Count - 1].Data.Value - oneBoxNorm >= ceil(closeStock[i], oneBoxNorm))
                        {
                            PointAndFigureData data = pfData[pfData.Count - 1];
                            pfData.RemoveAt(pfData.Count - 1);
                            data.Data = new KeyValuePair<double, double>(data.Data.Key, ceil(closeStock[i], oneBoxNorm));
                            pfData.Add(data);
                        }
                        //反転する場合
                        else if (pfData[pfData.Count - 1].Data.Value + oneBoxNorm * (reversePoint + 1) <= floor(closeStock[i], oneBoxNorm))
                        {
                            PointAndFigureData data = new PointAndFigureData();
                            data.Data = new KeyValuePair<double, double>(pfData[pfData.Count - 1].Data.Value + oneBoxNorm, floor(closeStock[i], oneBoxNorm));
                            pfData.Add(data);
                        }
                    }
                    else if (pfData[pfData.Count - 1].BuyorSell == Sign.TooSell)
                    {
                        //更に上がる場合
                        if (pfData[pfData.Count - 1].Data.Value + oneBoxNorm <= floor(closeStock[i], oneBoxNorm))
                        {
                            PointAndFigureData data = pfData[pfData.Count - 1];
                            pfData.RemoveAt(pfData.Count - 1);
                            data.Data = new KeyValuePair<double, double>(data.Data.Key, floor(closeStock[i], oneBoxNorm));
                            pfData.Add(data);
                        }
                        //反落する場合
                        else if (pfData[pfData.Count - 1].Data.Value - oneBoxNorm * (reversePoint + 1) >= ceil(closeStock[i], oneBoxNorm))
                        {
                            PointAndFigureData data = new PointAndFigureData();
                            data.Data = new KeyValuePair<double, double>(pfData[pfData.Count - 1].Data.Value - oneBoxNorm, ceil(closeStock[i], oneBoxNorm));
                            pfData.Add(data);
                        }
                    }
                }
            }
        }
        private double getOnePoint()
        {
            return 0.05;
        }


        /// <summary>
        /// nをmの倍数にして切り上げる
        /// </summary>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        private double ceil(double n, double m)
        {
            if (n % m == 0)
                return n;
            else
                return (((int)(n / m)) + 1) * m;
        }


        /// <summary>
        /// nをmの倍数にして切り上げる
        /// </summary>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        private double floor(double n, double m)
        {
            if (n % m == 0)
                return n;
            else
                return ((int)(n / m)) * m;
        }

        class PointAndFigureData
        {
            public KeyValuePair<double, double> Data;
            public Sign BuyorSell
            {
                get
                {
                    return Data.Key > Data.Value ? Sign.TooBuy : Sign.TooSell;
                }
            }
        }
    }
}
