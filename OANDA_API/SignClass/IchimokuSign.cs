using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    public class IchimokuSign
    {
        private List<double> closeStock;
        List<double> conversionLine;
        List<double> referenceLine;
        List<double> precSpan1;
        List<double> precSpan2;
        List<double> descSpan;
        int dataNum;

        public IchimokuSign(List<double> highStock, List<double> lowStock, List<double> closeStock)
        {
            conversionLine = new List<double>();
            referenceLine = new List<double>();
            precSpan1 = new List<double>();
            precSpan2 = new List<double>();
            descSpan = new List<double>();

            this.closeStock = closeStock;

            dataNum = closeStock.Count;

            double maxVal = highStock.Max();
            double minVal = lowStock.Min();

            for (int i = 0; i < dataNum; i++)
            {
                //転換線
                if (i < 8)
                {
                    conversionLine.Add(0);
                }
                else
                {
                    double min = double.MaxValue;
                    double max = double.MinValue;
                    for (int j = 0; j < 9; j++)
                    {
                        if (max < highStock[i - j])
                            max = highStock[i - j];
                        if (min > lowStock[i - j])
                            min = lowStock[i - j];
                    }
                    conversionLine.Add((max + min) / 2);
                }

                //基準線
                if (i < 25)
                {
                    referenceLine.Add(0);
                }
                else
                {
                    double min = double.MaxValue;
                    double max = double.MinValue;
                    for (int j = 0; j < 26; j++)
                    {
                        if (max < highStock[i - j])
                            max = highStock[i - j];
                        if (min > lowStock[i - j])
                            min = lowStock[i - j];
                    }
                    referenceLine.Add((max + min) / 2);
                }

                //先行スパン2の準備
                if (i < 51)
                {
                    precSpan2.Add(0);
                }
                else
                {
                    double min = double.MaxValue;
                    double max = double.MinValue;
                    for (int j = 0; j < 52; j++)
                    {
                        if (max < highStock[i - j])
                            max = highStock[i - j];
                        if (min > lowStock[i - j])
                            min = lowStock[i - j];
                    }
                    precSpan2.Add((max + min) / 2);
                }
            }

            double[] zeroNull = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            for (int i = 0; i < conversionLine.Count; i++)
            {
                if (conversionLine[i] == 0 || referenceLine[i] == 0)
                    precSpan1.Add(0);
                else
                    precSpan1.Add((conversionLine[i] + referenceLine[i]) / 2);
            }

            precSpan1.Reverse();
            precSpan1.AddRange(zeroNull);
            precSpan1.Reverse();

            precSpan2.Reverse();
            precSpan2.AddRange(zeroNull);
            precSpan2.Reverse();

            conversionLine.AddRange(zeroNull);
            referenceLine.AddRange(zeroNull);

            //遅行スパン
            descSpan.AddRange(closeStock.Skip(26));
            descSpan.AddRange(zeroNull);
            descSpan.AddRange(zeroNull);

            ////ここでdataNumの数をずらした分に変更
            dataNum = conversionLine.Count;

            //PrintChart();
        }

        public Sign GetSign(int date)
        {
            //売られすぎ買われすぎサインの判断
            if ((conversionLine[date - 1 - 10] > referenceLine[date - 1 - 10] && conversionLine[date - 1] < referenceLine[date - 1]) || (closeStock[date - 1 - 10] > Math.Max(precSpan1[date - 1 - 10], precSpan2[date - 1 - 10]) && closeStock[date - 1] < Math.Min(precSpan1[date - 1 - 10], precSpan2[date - 1])))
            {
                //売りサイン
                return Sign.TooBuy;
            }
            else if ((conversionLine[date - 1 - 10] < referenceLine[date - 1 - 10] && conversionLine[date - 1] > referenceLine[date - 1]) || (closeStock[date - 1 - 10] < Math.Min(precSpan1[date - 1 - 10], precSpan2[date - 1 - 10]) && closeStock[date - 1] > Math.Max(precSpan1[date - 1], precSpan2[date - 1])))
            {
                //買いサイン
                return Sign.TooSell;
            }
            else
            {
                return Sign.Normal;
            }
        }
    }
}
