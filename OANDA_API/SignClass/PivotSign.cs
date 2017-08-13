using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OANDA_API.SignClass
{
    public class PivotSign
    {
        private List<double> pivotVal;
        private List<double> B1Val;
        private List<double> B2Val;
        private List<double> S1Val;
        private List<double> S2Val;
        private List<double> HBOPVal;
        private List<double> LBOPVal;

        private List<double> closeStock;
        int dataNum;

        public PivotSign( List<double> highStock, List<double> lowStock, List<double> closeStock)
        {

            pivotVal = new List<double>();
            B1Val = new List<double>();
            B2Val = new List<double>();
            S1Val = new List<double>();
            S2Val = new List<double>();
            HBOPVal = new List<double>();
            LBOPVal = new List<double>();

            //各種線の準備
            this.closeStock = closeStock;

            dataNum = closeStock.Count;
            pivotVal.Add(0);
            B1Val.Add(0);
            B2Val.Add(0);
            S1Val.Add(0);
            S2Val.Add(0);
            HBOPVal.Add(0);
            LBOPVal.Add(0);

            for (int i = 1; i < dataNum; i++)
            {
                pivotVal.Add((highStock[i - 1] + lowStock[i - 1] + closeStock[i - 1]) / 3);
                B1Val.Add(pivotVal[i] * 2 - highStock[i - 1]);
                B2Val.Add(pivotVal[i] - highStock[i - 1] + lowStock[i - 1]);
                S1Val.Add(pivotVal[i] * 2 - lowStock[i - 1]);
                S2Val.Add(pivotVal[i] + highStock[i - 1] - lowStock[i - 1]);
                HBOPVal.Add(S1Val[i] + highStock[i - 1] - lowStock[i - 1]);
                LBOPVal.Add(B1Val[i] - (highStock[i - 1] - lowStock[i - 1]));
            }
        }

        public Sign GetSign(int date)
        {
            //売られすぎ買われすぎサインの判断
            if (closeStock[date - 1] < B2Val[date - 1])
            {
                return Sign.TooSell;
            }
            if (closeStock[date - 1] > S1Val[date - 1])
            {
                return Sign.TooBuy;
            }
            else
            {
                return Sign.Normal;
            }
        }
    }
}
