using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TechTool
{
    public class SignData
    {
        public string codeName
        {
            set;
            get;
        }

        public int codeNumber
        {
            set;
            get;
        }

        public Sign maSign
        {
            set;
            get;
        }

        public Sign bollingerSign
        {
            set;
            get;
        }

        public Sign ichimokuSign
        {
            set;
            get;
        }

        public Sign parabolicSign
        {
            set;
            get;
        }

        public Sign pivotSign
        {
            set;
            get;
        }

        public Sign ROCSign
        {
            set;
            get;
        }

        public Sign psychologicalSign
        {
            set;
            get;
        }

        public Sign deviationSign
        {
            set;
            get;
        }

        public Sign RSISign
        {
            set;
            get;
        }

        public Sign RCISign
        {
            set;
            get;
        }

        public Sign perRSign
        {
            set;
            get;
        }

        public Sign stochasticSign
        {
            set;
            get;
        }

        public Sign MACDSign
        {
            set;
            get;
        }

        public Sign DMISign
        {
            set;
            get;
        }

        public Sign VolumeSign
        {
            set;
            get;
        }

        public Sign MFISign
        {
            set;
            get;
        }

        public int BuySignCount
        {
            set;
            get;
        }

        public int SellSignCount
        {
            set;
            get;
        }

        public int DiffCount
        {
            set;
            get;
        }

        public Trend MATrend
        {
            set;
            get;
        }

        public Trend DMITrend
        {
            set;
            get;
        }

        public Trend ParabolicTrend
        {
            set;
            get;
        }

        public Trend MACDTrend
        {
            set;
            get;
        }

        public Sign HLBand { set; get; }

        public Sign TwoMA { set; get; }

        public Sign FourWeeksRule { set; get; }

        public Sign PFB1 { set; get; }

        public Sign PFB2 { set; get; }

        public Sign PFB3 { set; get; }

        public Sign PFB4 { set; get; }

        public Sign PFB5 { set; get; }

        public Sign PFS1 { set; get; }

        public Sign PFS2 { set; get; }

        public Sign WhiteMaruBozu { set; get; }

        public Sign WhiteOhbikeBozu { set; get; }

        public Sign WhiteYoritsukiBozu { set; get; }

        public Sign BlackMaruBozu { set; get; }

        public Sign BlackOhbikeBozu { set; get; }

        public Sign BlackYoritsukiBozu { set; get; }

        public Sign WhiteShitakage { set; get; }

        public Sign WhiteUekage { set; get; }

        public Sign WhiteKarakasa { set; get; }

        public Sign BlackShitakage { set; get; }

        public Sign BlackUekage { set; get; }

        public Sign BlackKarakasa { set; get; }

        public Sign Tonkachi { set; get; }

        public Sign Tombo { set; get; }

        public Sign Touba { set; get; }

        public Sign SankuTatakikomi { set; get; }

        public Sign SanteDaiInsen { set; get; }

        public Sign LastIdakiInsen { set; get; }

        public Sign AkenoMyojo { set; get; }

        public Sign Sutegozoko { set; get; }

        public Sign TakuriSen { set; get; }

        public Sign Seiryokusen { set; get; }

        public Sign BlackBlackHarami { set; get; }

        public Sign IdakiYosen { set; get; }

        public Sign Akasanpei { set; get; }

        public Sign KaiFiveYosen { set; get; }

        public Sign Osaekomisen { set; get; }

        public Sign Agesanpo { set; get; }

        public Sign SankuHumiage { set; get; }

        public Sign SinneHatteRiguiSen { set; get; }

        public Sign IkiDumari { set; get; }

        public Sign JouiUwabanareInsen { set; get; }

        public Sign YoYoHarami { set; get; }

        public Sign LastIdakiYosen { set; get; }

        public Sign IdakiInsen { set; get; }

        public double minPurchasePrice
        {
            set;
            get;
        }

        public double tenAfterPrice { set; get; }

        public double AMonthAfterPrice { set; get; }

        public double VolumeRatio { set; get; }

        public double VolumeAmount { set; get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="codeName"></param>
        public SignData(string codeName,int codeNumber,Sign ma,Sign bollinger,Sign ichimoku,Sign parabolic,Sign pivot,Sign ROC,Sign psycho,Sign deviation,
            Sign RSI,Sign RCI,Sign perR,Sign stochastic,Sign MACD,Sign DMI,Sign volume,Sign MFI,double minPP)
        {
            this.codeName = codeName;
            this.codeNumber = codeNumber;
            this.maSign = ma;
            this.bollingerSign = bollinger;
            this.ichimokuSign = ichimoku;
            this.parabolicSign = parabolic;
            this.pivotSign = pivot;
            this.ROCSign = ROC;
            this.psychologicalSign = psycho;
            this.deviationSign = deviation;
            this.RSISign = RSI;
            this.RCISign = RCI;
            this.perRSign = perR;
            this.stochasticSign = stochastic;
            this.MACDSign = MACD;
            this.DMISign = DMI;
            this.VolumeSign = volume;
            this.MFISign = MFI;
            this.minPurchasePrice = minPP;

            Type t = typeof(SignData);
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo info in properties)
            {
                object obj = info.GetValue(this);

                if (obj is Sign)
                {
                    Sign sign = (Sign)obj;
                    
                    if (sign == Sign.TooSell)
                    {
                        BuySignCount++;
                    }
                    else if (sign == Sign.TooBuy)
                    {
                        SellSignCount++;
                    }
                }
            }

            this.DiffCount = BuySignCount - SellSignCount;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="codeName"></param>
        /// <param name="codeNumber"></param>
        /// <param name="ma"></param>4
        /// <param name="bollinger"></param>5
        /// <param name="ichimoku"></param>
        /// <param name="parabolic"></param>6
        /// <param name="pivot"></param>7
        /// <param name="ROC"></param>8
        /// <param name="psycho"></param>9
        /// <param name="deviation"></param>10
        /// <param name="RSI"></param>
        /// <param name="RCI"></param>
        /// <param name="perR"></param>
        /// <param name="stochastic"></param>
        /// <param name="MACD"></param>15
        /// <param name="DMI"></param>
        /// <param name="volume"></param>
        /// <param name="MFI"></param>
        /// <param name="maTrend"></param>
        /// <param name="DMITrend"></param>20
        /// <param name="ParabolicTrend"></param>
        /// <param name="MACDTrend"></param>
        /// <param name="hlBand"></param>
        /// <param name="twoMA"></param>
        /// <param name="fourWeeksRule"></param>
        /// <param name="pfB1"></param>
        /// <param name="pfB2"></param>
        /// <param name="pfB3"></param>
        /// <param name="pfB4"></param>
        /// <param name="pfB5"></param>30
        /// <param name="pfS1"></param>
        /// <param name="pfS2"></param>
        /// <param name="WhiteMaruBozu"></param>
        /// <param name="WhiteOhBikeBozu"></param>
        /// <param name="WhiteYoritsukiBozu"></param>
        /// <param name="BlackMaruBozu"></param>
        /// <param name="BlackOhbikeBozu"></param>
        /// <param name="BlackYoritsukiBozu"></param>
        /// <param name="WhiteShitakage"></param>
        /// <param name="WhiteUekage"></param>
        /// <param name="WhiteKarakasa"></param>
        /// <param name="BlackShitakage"></param>
        /// <param name="BlackUekage"></param>
        /// <param name="BlackKarakasa"></param>
        /// <param name="Tonkachi"></param>
        /// <param name="Tombo"></param>
        /// <param name="Touba"></param>
        /// <param name="SankuTatamikomi"></param>
        /// <param name="SanteDaiInsen"></param>
        /// <param name="LastIdakiInsen"></param>
        /// <param name="Akenomyojo"></param>
        /// <param name="SUtegozoko"></param>
        /// <param name="TakuriSen"></param>
        /// <param name="SeiryokuSen"></param>
        /// <param name="InInHarami"></param>
        /// <param name="IdakiYosen"></param>
        /// <param name="Akasanpei"></param>
        /// <param name="KaiFiveYosen"></param>
        /// <param name="OsaekomiSen"></param>
        /// <param name="Agesanpo"></param>
        /// <param name="SankuHumiage"></param>
        /// <param name="SinneHatteRiguiSen"></param>
        /// <param name="IkiDumari"></param>
        /// <param name="JouiUwabanareInsen"></param>
        /// <param name="YoYoHarami"></param>
        /// <param name="LastIdakiYosen"></param>
        /// <param name="IdakiInsen"></param>
        /// <param name="minPP"></param>
        public SignData(string codeName, int codeNumber, Sign ma, Sign bollinger, Sign ichimoku, Sign parabolic, Sign pivot, Sign ROC, Sign psycho, Sign deviation,
            Sign RSI, Sign RCI, Sign perR, Sign stochastic, Sign MACD, Sign DMI, Sign volume, Sign MFI,
            Trend maTrend,Trend DMITrend,Trend ParabolicTrend,Trend MACDTrend, 
            Sign hlBand, Sign twoMA, Sign fourWeeksRule, Sign pfB1, Sign pfB2, Sign pfB3, Sign pfB4, Sign pfB5, Sign pfS1, Sign pfS2,
            Sign WhiteMaruBozu,Sign WhiteOhBikeBozu, Sign WhiteYoritsukiBozu,Sign BlackMaruBozu,Sign BlackOhbikeBozu, Sign BlackYoritsukiBozu,Sign WhiteShitakage , Sign WhiteUekage,Sign WhiteKarakasa,
            Sign BlackShitakage,Sign BlackUekage,Sign BlackKarakasa, Sign Tonkachi,Sign Tombo, Sign Touba,
            Sign SankuTatamikomi,Sign SanteDaiInsen,Sign LastIdakiInsen, Sign Akenomyojo, Sign SUtegozoko, Sign TakuriSen, Sign SeiryokuSen, Sign InInHarami, Sign IdakiYosen,
            Sign Akasanpei, Sign KaiFiveYosen, Sign OsaekomiSen, Sign Agesanpo,
            Sign SankuHumiage, Sign SinneHatteRiguiSen,Sign IkiDumari, Sign JouiUwabanareInsen,Sign YoYoHarami, Sign LastIdakiYosen, Sign IdakiInsen
            ,double minPP)
        {
            this.codeName = codeName;
            this.codeNumber = codeNumber;
            this.maSign = ma;
            this.bollingerSign = bollinger;
            this.ichimokuSign = ichimoku;
            this.parabolicSign = parabolic;
            this.pivotSign = pivot;
            this.ROCSign = ROC;
            this.psychologicalSign = psycho;
            this.deviationSign = deviation;
            this.RSISign = RSI;
            this.RCISign = RCI;
            this.perRSign = perR;
            this.stochasticSign = stochastic;
            this.MACDSign = MACD;
            this.DMISign = DMI;
            this.VolumeSign = volume;
            this.MFISign = MFI;
            this.MATrend = maTrend;
            this.DMITrend = DMITrend;
            this.ParabolicTrend = ParabolicTrend;
            this.MACDTrend = MACDTrend;

            this.HLBand = hlBand;
            this.TwoMA = twoMA;
            this.FourWeeksRule = fourWeeksRule;
            this.PFB1 = pfB1;
            this.PFB2 = pfB2;
            this.PFB3 = pfB3;
            this.PFB4 = pfB4;
            this.PFB5 = pfB5;
            this.PFS1 = pfS1;
            this.PFS2 = pfS2;

            this.WhiteMaruBozu = WhiteMaruBozu;
            this.WhiteOhbikeBozu = WhiteOhBikeBozu;
            this.WhiteYoritsukiBozu = WhiteYoritsukiBozu;
            this.BlackMaruBozu = BlackMaruBozu;
            this.BlackOhbikeBozu = BlackOhbikeBozu;
            this.BlackYoritsukiBozu = BlackYoritsukiBozu;
            this.WhiteShitakage = WhiteShitakage;
            this.WhiteUekage = WhiteUekage;
            this.WhiteKarakasa = WhiteKarakasa;
            this.BlackShitakage = BlackShitakage;
            this.BlackUekage = BlackUekage;
            this.BlackKarakasa = BlackKarakasa;
            this.Tonkachi = Tonkachi;
            this.Tombo = Tombo;
            this.Touba = Touba;
            this.SankuTatakikomi = SankuTatamikomi;
            this.SanteDaiInsen = SanteDaiInsen;
            this.LastIdakiInsen = LastIdakiInsen;
            this.AkenoMyojo = Akenomyojo;
            this.Sutegozoko = SUtegozoko;
            this.TakuriSen = TakuriSen;
            this.Seiryokusen = SeiryokuSen;
            this.BlackBlackHarami = InInHarami;
            this.IdakiYosen = IdakiYosen;
            this.Akasanpei = Akasanpei;
            this.KaiFiveYosen = KaiFiveYosen;
            this.Osaekomisen = OsaekomiSen;
            this.Agesanpo = Agesanpo;
            this.SankuHumiage = SankuHumiage;
            this.SinneHatteRiguiSen = SinneHatteRiguiSen;
            this.IkiDumari = IkiDumari;
            this.JouiUwabanareInsen = JouiUwabanareInsen;
            this.YoYoHarami = YoYoHarami;
            this.LastIdakiYosen = LastIdakiYosen;
            this.IdakiYosen = IdakiYosen;
            
            this.minPurchasePrice = minPP;

            UpdateCount();
        }


        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public SignData()
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        public SignData(string code)
        {
            this.codeNumber = int.Parse(code);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(maSign.ToString("D") + " ,");
            builder.Append(bollingerSign.ToString("D") + " ,");
            builder.Append(ichimokuSign.ToString("D") + " ,");
            builder.Append(parabolicSign.ToString("D") + " ,");
            builder.Append(pivotSign.ToString("D") + " ,");
            builder.Append(ROCSign.ToString("D") + " ,");
            builder.Append(psychologicalSign.ToString("D") + " ,");
            builder.Append(deviationSign.ToString("D") + " ,");
            builder.Append(RSISign.ToString("D") + " ,");
            builder.Append(RCISign.ToString("D") + " ,");
            builder.Append(perRSign.ToString("D") + " ,");
            builder.Append(stochasticSign.ToString("D") + " ,");
            builder.Append(MACDSign.ToString("D") + " ,");
            builder.Append(DMISign.ToString("D") + " ,");
            builder.Append(VolumeSign.ToString("D") + " ,");
            builder.Append(MFISign.ToString("D") + " ,");
            builder.Append(MATrend.ToString("D") + " ,");
            builder.Append(DMITrend.ToString("D") + " ,");
            builder.Append(ParabolicTrend.ToString("D") + " ,");
            builder.Append(MACDTrend.ToString("D") + " ,");
            builder.Append(HLBand.ToString("D") + " ,");
            builder.Append(TwoMA.ToString("D") + " ,");
            builder.Append(FourWeeksRule.ToString("D") + " ,");
            builder.Append(PFB1.ToString("D") + " ,");
            builder.Append(PFB2.ToString("D") + " ,");
            builder.Append(PFB3.ToString("D") + " ,");
            builder.Append(PFB4.ToString("D") + " ,");
            builder.Append(PFB5.ToString("D") + " ,");
            builder.Append(PFS1.ToString("D") + " ,");
            builder.Append(PFS2.ToString("D") + " ,");
            builder.Append(minPurchasePrice + " ,");
            builder.Append( "NULL" + " ,");
            builder.Append(WhiteMaruBozu.ToString("D") + " ,");
            builder.Append(WhiteOhbikeBozu.ToString("D") + " ,");
            builder.Append(WhiteYoritsukiBozu.ToString("D") + " ,");
            builder.Append(BlackMaruBozu.ToString("D") + " ,");
            builder.Append(BlackOhbikeBozu.ToString("D") + " ,");
            builder.Append(BlackYoritsukiBozu.ToString("D") + " ,");
            builder.Append(WhiteShitakage.ToString("D") + " ,");
            builder.Append(WhiteUekage.ToString("D") + " ,");
            builder.Append(WhiteKarakasa.ToString("D") + " ,");
            builder.Append(BlackShitakage.ToString("D") + " ,");
            builder.Append(BlackUekage.ToString("D") + " ,");
            builder.Append(BlackKarakasa.ToString("D") + " ,");
            builder.Append(Tonkachi.ToString("D") + " ,");
            builder.Append(Tombo.ToString("D") + " ,");
            builder.Append(Touba.ToString("D") + " ,");
            builder.Append(SankuTatakikomi.ToString("D") + " ,");
            builder.Append(SanteDaiInsen.ToString("D") + " ,");
            builder.Append(LastIdakiInsen.ToString("D") + " ,");
            builder.Append(AkenoMyojo.ToString("D") + " ,");
            builder.Append(Sutegozoko.ToString("D") + " ,");
            builder.Append(TakuriSen.ToString("D") + " ,");
            builder.Append(Seiryokusen.ToString("D") + " ,");
            builder.Append(BlackBlackHarami.ToString("D") + " ,");
            builder.Append(IdakiYosen.ToString("D") + " ,");
            builder.Append(Akasanpei.ToString("D") + " ,");
            builder.Append(KaiFiveYosen.ToString("D") + " ,");
            builder.Append(Osaekomisen.ToString("D") + " ,");
            builder.Append(Agesanpo.ToString("D") + " ,");
            builder.Append(SankuHumiage.ToString("D") + " ,");
            builder.Append(SinneHatteRiguiSen.ToString("D") + " ,");
            builder.Append(IkiDumari.ToString("D") + " ,");
            builder.Append(JouiUwabanareInsen.ToString("D") + " ,");
            builder.Append(YoYoHarami.ToString("D") + " ,");
            builder.Append(LastIdakiYosen.ToString("D") + " ,");
            builder.Append(IdakiInsen.ToString("D") + " ,");
            builder.Append(VolumeRatio + " ,");
            builder.Append(VolumeAmount + " ,");
            builder.Append("'" + DateTime.Today + "'");

            return builder.ToString();
        }

        public void UpdateCount()
        {
            Type t = typeof(SignData);
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo info in properties)
            {
                object obj = info.GetValue(this);

                if (obj is Sign)
                {
                    Sign sign = (Sign)obj;

                    if (sign == Sign.TooSell)
                    {
                        this.BuySignCount++;
                    }
                    else if (sign == Sign.TooBuy)
                    {
                        this.SellSignCount++;
                    }
                }
            }

            this.DiffCount = BuySignCount - SellSignCount;
        }

        public static SignData CreateSignData(DataRow row)
        {
            SignData sd = new SignData(
                    "",
                    int.Parse(row[0].ToString()),
                    (Sign)int.Parse(row[4].ToString()),
                    (Sign)int.Parse(row[5].ToString()),
                    Sign.Normal,
                    (Sign)int.Parse(row[6].ToString()),
                    (Sign)int.Parse(row[7].ToString()),
                    (Sign)int.Parse(row[8].ToString()),
                    (Sign)int.Parse(row[9].ToString()),
                    (Sign)int.Parse(row[10].ToString()),
                    (Sign)int.Parse(row[11].ToString()),
                    (Sign)int.Parse(row[12].ToString()),
                    row[13].ToString() == "" ? Sign.Normal : (Sign)int.Parse(row[13].ToString()),
                    (Sign)int.Parse(row[14].ToString()),
                    (Sign)int.Parse(row[15].ToString()),
                    (Sign)int.Parse(row[16].ToString()),
                    (Sign)int.Parse(row[17].ToString()),
                    (Sign)int.Parse(row[18].ToString()),
                    (Trend)int.Parse(row[19].ToString()),
                    (Trend)int.Parse(row[20].ToString()),
                    (Trend)int.Parse(row[21].ToString()),
                    (Trend)int.Parse(row[22].ToString()),
                    (Sign)int.Parse(row[23].ToString()),
                    (Sign)int.Parse(row[24].ToString()),
                    (Sign)int.Parse(row[25].ToString()),
                    (Sign)int.Parse(row[26].ToString()),
                    (Sign)int.Parse(row[27].ToString()),
                    (Sign)int.Parse(row[28].ToString()),
                    (Sign)int.Parse(row[29].ToString()),
                    (Sign)int.Parse(row[30].ToString()),
                    (Sign)int.Parse(row[31].ToString()),
                    (Sign)int.Parse(row[32].ToString()),

                    (Sign)int.Parse(row[35].ToString()),
                    (Sign)int.Parse(row[36].ToString()),
                    (Sign)int.Parse(row[37].ToString()),
                    (Sign)int.Parse(row[38].ToString()),
                    (Sign)int.Parse(row[39].ToString()),
                    (Sign)int.Parse(row[40].ToString()),
                    (Sign)int.Parse(row[41].ToString()),
                    (Sign)int.Parse(row[42].ToString()),
                    (Sign)int.Parse(row[43].ToString()),
                    (Sign)int.Parse(row[44].ToString()),
                    (Sign)int.Parse(row[45].ToString()),
                    (Sign)int.Parse(row[46].ToString()),
                    (Sign)int.Parse(row[47].ToString()),
                    (Sign)int.Parse(row[48].ToString()),
                    (Sign)int.Parse(row[49].ToString()),
                    (Sign)int.Parse(row[50].ToString()),
                    (Sign)int.Parse(row[51].ToString()),
                    (Sign)int.Parse(row[52].ToString()),
                    (Sign)int.Parse(row[53].ToString()),
                    (Sign)int.Parse(row[54].ToString()),
                    (Sign)int.Parse(row[55].ToString()),
                    (Sign)int.Parse(row[56].ToString()),
                    (Sign)int.Parse(row[57].ToString()),
                    (Sign)int.Parse(row[58].ToString()),
                    (Sign)int.Parse(row[59].ToString()),
                    (Sign)int.Parse(row[60].ToString()),
                    (Sign)int.Parse(row[61].ToString()),
                    (Sign)int.Parse(row[62].ToString()),
                    (Sign)int.Parse(row[63].ToString()),
                    (Sign)int.Parse(row[64].ToString()),
                    (Sign)int.Parse(row[65].ToString()),
                    (Sign)int.Parse(row[66].ToString()),
                    (Sign)int.Parse(row[67].ToString()),
                    (Sign)int.Parse(row[68].ToString()),
                    (Sign)int.Parse(row[69].ToString()),
                    double.Parse(row[33].ToString())
                    );
            double price = 0.0;
            double.TryParse(row[34].ToString(), out price);
            sd.tenAfterPrice = price;
            sd.VolumeRatio = double.Parse(row[73].ToString());
            sd.VolumeAmount = double.Parse(row[74].ToString());
            try
            {
                sd.AMonthAfterPrice = double.Parse(row[71].ToString());
            }
            catch (Exception)
            {
                sd.AMonthAfterPrice = 0;
            }

            return sd;
        }
    }
}
