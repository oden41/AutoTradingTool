using RestApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using RestApi.DataTypes;
using OANDA_API.SignClass;
using System.Threading;

namespace OANDA_API
{
    class AutoFX
    {
        /// <summary>
        /// id,unit,sellDate
        /// </summary>
        private List<Tuple<long, int, DateTime>> orderList;

        private DataBase DB { get; set; }

        private int AccId { get; set; }

        private string Strategy = "1 == 1";

        public AutoFX()
        {
            DB = new DataBase();
            AccId = int.Parse(ConfigurationManager.AppSettings["AccountId"]);
        }


        /// <summary>
        /// 自動トレード開始
        /// </summary>
        public void Start()
        {
            //営業時間まで待機
            while (!IsOpenMarket())
                Thread.Sleep(1000);

            while (IsOpenMarket())
            {
                //10分毎(分が10の倍数)まで待機
                while (DateTime.Today.Minute % 10 != 0)
                    Thread.Sleep(1000);

                //売るタイミングでポジションが残ってるなら決済
                foreach(var order in orderList.Where(data => data.Item3 >= DateTime.Now))
                {

                }

                if (GetCandle())
                {
                    //戦略にヒット
                }
            }

        }

        public bool IsOpenMarket()
        {
            //夏・冬時間
            int start = DateTime.Today.Month >= 3 || DateTime.Today.Month <= 11 ? 6 : 7;
            int end = DateTime.Today.Month >= 3 || DateTime.Today.Month <= 11 ? 18 : 19;
            return DateTime.Today.DayOfWeek != DayOfWeek.Sunday || (DateTime.Today.DayOfWeek == DayOfWeek.Saturday && DateTime.Today.Hour < end) || (DateTime.Today.DayOfWeek == DayOfWeek.Monday && DateTime.Today.Hour >= start);
        }


        /// <summary>
        /// 直近dataCount本のデータを取得し，DBに追加
        /// </summary>
        private bool GetCandle(int dataCount = 500)
        {
            var result = Rest.GetCandles("USD_JPY", AccId, count: dataCount);
            var sql = "SELECT MAX(time) FROM USDJPY_10m";
            var lastDate = DB.ExecuteScalar(sql, DateTime.MinValue);
            foreach (var data in result.Where(data => data.time > lastDate))
            {
                var insertSQL = $@"
INSERT INTO USDJPY_10m(
time,
openBid,
openAsk,
highBid,
highAsk,
lowBid,
lowAsk,
closeBid,
closeAsk,
openMid,
highMid,
lowMid,
closeMid
)
VALUES
(
'{data.time.ToString("yyyy-MM-dd HH:mm:ss")}',
{data.openBid},
{data.openAsk},
{data.highBid},
{data.highAsk},
{data.lowBid},
{data.lowAsk},
{data.closeBid},
{data.closeAsk},
{data.openMid},
{data.highMid},
{data.lowMid},
{data.closeMid}
)
";
                DB.ExecuteNonQuery(insertSQL);
            }
            //テクニカル指標生成
            return MakeIndex(result);
        }


        /// <summary>
        /// resultからテクニカル指標を生成し，戦略にヒットするかを返す
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool MakeIndex(List<Candle> result)
        {
            var highStock = new List<double>(result.Select(row => row.highMid));
            var lowStock = new List<double>(result.Select(row => row.lowMid));
            var openStock = new List<double>(result.Select(row => row.openMid));
            var closeStock = new List<double>(result.Select(row => row.closeMid));
            var dateStock = new List<DateTime>(result.Select(row => row.time));

            var ma = new MASign(closeStock);

            var bollinger = new BollingerSign(closeStock);

            var ichimoku = new IchimokuSign(highStock, lowStock, closeStock);

            var parabolic = new ParabolicSign(highStock, lowStock, closeStock);

            var pivot = new PivotSign(highStock, lowStock, closeStock);

            var roc = new ROCSign(closeStock);

            var psychological = new PsychologicalSign(closeStock);

            var deviation = new DeviationSign(closeStock);

            var rsi = new RSISign(closeStock);

            var rci = new RCISign(closeStock);

            var perR = new PerRSign(highStock, lowStock, closeStock);

            var stochastic = new StochasticSign(highStock, lowStock, closeStock);

            var macd = new MACDSign(closeStock);

            var dmi = new DMISign(highStock, lowStock, closeStock);

            //詳細画面
            var hlBand = new HaLBandSign(highStock, lowStock, closeStock);

            var twoMA = new TwoMASign(closeStock);

            var fourWeekRule = new FourWeeksRuleSign(highStock, lowStock, closeStock);

            var pfSign = new PointAndFigureSign(closeStock);

            var B1 = pfSign.isB1Sign() ? Sign.TooSell : Sign.Normal;
            var B2 = pfSign.isB2Sign() ? Sign.TooSell : Sign.Normal;
            var B3 = pfSign.isB3Sign() ? Sign.TooSell : Sign.Normal;
            var B4 = pfSign.isB4Sign() ? Sign.TooSell : Sign.Normal;
            var B5 = pfSign.isB5Sign() ? Sign.TooSell : Sign.Normal;
            var S1 = pfSign.isS1Sign() ? Sign.TooBuy : Sign.Normal;
            var S2 = pfSign.isS2Sign() ? Sign.TooBuy : Sign.Normal;

            var sql =
                  @"INSERT INTO IndexData_10m_2017(
AsOfDate,
MASign,
BollingerSign,
IchimokuSign,
ParabolicSign,
PivotSign,
ROCSign,
PsychologicalSign,
DeviationSign,
RSISign,
RCISign,
perRSign,
StochasticSign,
MACDSign,
DMISign,
MATrend,
DMITrend,
ParabolicTrend,
MACDTrend,
HLBand,
TwoMA,
FourWeeksRule,
PFB1,
PFB2,
PFB3,
PFB4,
PFB5,
PFS1,
PFS2,
NextBid,
NextAsk,
TenAfterBid,
TenAfterAsk,
TenMaxDDBid,
TenMaxDDAsk,
TenMaxDUBid,
TenMaxDUAsk,
RegisterDate
) VALUES(
" + "'" + dateStock[dateStock.Count - 1].ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," +
  ma.GetSign(closeStock.Count).ToString("D") + "," +
  bollinger.GetSign(closeStock.Count).ToString("D") + "," +
  ichimoku.GetSign(closeStock.Count).ToString("D") + "," +
  parabolic.GetSign(closeStock.Count).ToString("D") + "," +
  pivot.GetSign(closeStock.Count).ToString("D") + "," +
  roc.GetSign(closeStock.Count).ToString("D") + "," +
  psychological.GetSign(closeStock.Count).ToString("D") + "," +
  deviation.GetSign(closeStock.Count).ToString("D") + "," +
  rsi.GetSign(closeStock.Count).ToString("D") + "," +
  rci.GetSign(closeStock.Count).ToString("D") + "," +
  perR.GetSign(closeStock.Count).ToString("D") + "," +
  stochastic.GetSign(closeStock.Count).ToString("D") + "," +
  macd.GetSign(closeStock.Count).ToString("D") + "," +
  dmi.GetSign(closeStock.Count).ToString("D") + "," +
  ma.GetTrend(closeStock.Count).ToString("D") + "," +
  dmi.GetTrend(closeStock.Count).ToString("D") + "," +
  parabolic.GetTrend(closeStock.Count).ToString("D") + "," +
  macd.GetTrend(closeStock.Count).ToString("D") + "," +
  hlBand.GetSign(closeStock.Count).ToString("D") + "," +
  twoMA.GetSign(closeStock.Count).ToString("D") + "," +
  fourWeekRule.GetSign(closeStock.Count).ToString("D") + "," +
  B1.ToString("D") + "," +
  B2.ToString("D") + "," +
  B3.ToString("D") + "," +
  B4.ToString("D") + "," +
  B5.ToString("D") + "," +
  S1.ToString("D") + "," +
  S2.ToString("D") + "," +
  DBNull.Value + "," +
  DBNull.Value + "," +
  DBNull.Value + "," +
  DBNull.Value + "," +
  DBNull.Value + "," +
  DBNull.Value + "," +
  DBNull.Value + "," +
  DBNull.Value + "," +
"'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
  ")";
            DB.ExecuteNonQuery(sql);

            var lastDate = dateStock.Last();
            var selectSQL = $"SELECT COUNT(AsOfDate) FROM IndexData_10m_2017 WHERE AsOfDate = '{lastDate.ToString("yyyy-MM-dd HH:mm:ss")}' AND {Strategy}";
            return DB.ExecuteScalar(selectSQL, 0) != 0;
        }


        /// <summary>
        /// 週末に呼び出すメソッド
        /// NULLとなっている価格データを埋める
        /// </summary>
        private void SetIndexData()
        {

        }
    }
}
