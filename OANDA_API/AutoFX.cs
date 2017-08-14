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
        /// 最大マージン
        /// </summary>
        private static readonly double MAX_MARGIN = 0.30;
        /// <summary>
        /// 1トレードあたりの最大マージン
        /// </summary>
        private static readonly double MAX_MARGIN_PERTRADE = 0.10;
        /// <summary>
        /// 売買の基準となるユニット数
        /// </summary>
        private static readonly int UNIT = 5000;
        /// <summary>
        /// レバレッジ，USD/JPYは0.04
        /// </summary>
        private static readonly double LEVERAGE = 0.04;

        private static readonly int SELLTERM = 10;

        /// <summary>
        /// id,sellDate
        /// </summary>
        private List<Tuple<long, DateTime>> orderList;

        private DataBase DB { get; set; }
        /// <summary>
        /// アカウントID
        /// </summary>
        private int AccId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private double Margin { get; set; }
        /// <summary>
        /// strategySQL,side
        /// </summary>
        private List<KeyValuePair<string,string>> strategy;


        public AutoFX()
        {
            DB = new DataBase();
            AccId = int.Parse(ConfigurationManager.AppSettings["AccountId"]);
            orderList = new List<Tuple<long, DateTime>>();
            strategy = new List<KeyValuePair<string, string>>();
            //strategy.Add(new KeyValuePair<string, string>("1=1", "buy"));
            strategy.Add(new KeyValuePair<string, string>("1=1", "sell"));
        }


        /// <summary>
        /// 自動トレード開始
        /// </summary>
        public void Start()
        {
            var lastCheck = DateTime.MinValue;
            //営業時間まで待機
            while (!IsOpenMarket())
                Thread.Sleep(1000);

            while (IsOpenMarket())
            {
                //10分毎(分が10の倍数)まで待機
                while (DateTime.Now.Minute % 10 != 0 || (DateTime.Now - lastCheck).TotalSeconds < 60 * 10 * 1.5)
                    Thread.Sleep(1000);

                //保持しているポジション一覧を取得
                var tradeList = Rest.GetTradeList(AccId);

                //覚えのないID -> orderListに追加
                var idList = tradeList.Where(item => !orderList.Select(tuple => tuple.Item1).Contains(item.id)).Select(item => item.id).ToList();
                idList.ForEach(item =>
                {
                    orderList.Add(Tuple.Create(item, DateTime.Now.AddMinutes(SELLTERM)));
                });

                //売るタイミングでポジションが残ってるなら決済
                foreach (var order in orderList.Where(data => data.Item2 <= DateTime.Now))
                {
                    //IDで照合
                    var pos = tradeList.Where(item => item.id == order.Item1).FirstOrDefault();
                    if(pos != null)
                    {
                        //反対売買を行う
                        Order(pos.units, pos.side == "buy" ? "sell" : "buy");
                    }
                }

                //データ取得，テクニカル指標生成
                var candles = new List<Candle>();
                //最新の足の取得には数秒ずれるため
                do
                {
                    candles = GetCandle();
                } while ((DateTime.Now - candles.Last().time).TotalSeconds >  60 * 10 * 1.5);
                var lastDate = MakeIndex(candles);
                foreach (var str in strategy)
                {
                    var selectSQL = $"SELECT COUNT(AsOfDate) FROM IndexData_10m_2017 WHERE AsOfDate = '{lastDate.ToString("yyyy-MM-dd HH:mm:ss")}' AND {str.Key}";
                    if (DB.ExecuteScalar(selectSQL, 0) != 0)
                    {
                        //marginを元にユニット数を設定
                        var units = 0;
                        var detail = Rest.GetAccountDetails(AccId);
                        var used = detail.marginUsed;
                        var denom = detail.balance + detail.unrealizedPl;
                        var nowMargin = detail.MarginLiquidationRate;
                        var margin = nowMargin;
                        var nowPrice = candles.Last().closeMid;
                        while (margin < nowMargin + MAX_MARGIN_PERTRADE && margin < MAX_MARGIN)
                        {
                            units += UNIT;
                            margin = (used + nowPrice * units) * LEVERAGE * 0.5 / (denom);
                        }
                        //注文
                        if (units > 0)
                        {
                            var id = Order(units, str.Value);
                            //orderListに追加
                            orderList.Add(Tuple.Create(id, lastDate.AddMinutes(SELLTERM)));
                        }
                    }
                }

                lastCheck = lastDate;
            }

        }


        /// <summary>
        /// 営業日かどうかを返す
        /// </summary>
        /// <returns></returns>
        private bool IsOpenMarket()
        {
            //夏・冬時間
            int start = DateTime.Today.Month >= 3 || DateTime.Today.Month <= 11 ? 6 : 7;
            int end = DateTime.Today.Month >= 3 || DateTime.Today.Month <= 11 ? 18 : 19;
            return DateTime.Today.DayOfWeek != DayOfWeek.Sunday || (DateTime.Today.DayOfWeek == DayOfWeek.Saturday && DateTime.Today.Hour < end) || (DateTime.Today.DayOfWeek == DayOfWeek.Monday && DateTime.Today.Hour >= start);
        }


        /// <summary>
        /// 注文メソッド
        /// </summary>
        /// <param name="units"></param>
        /// <param name="side"></param>
        /// <param name="instrument"></param>
        /// <returns></returns>
        private long Order(int units,string side,string instrument = "USD_JPY")
        {
            var dir = new Dictionary<string, string>();
            dir.Add("instrument", "USD_JPY");
            dir.Add("units", units.ToString());
            dir.Add("side", side);
            dir.Add("type", "market");
            return Rest.PostMarketOrder(AccId, dir);
        }


        /// <summary>
        /// 直近dataCount本のデータを取得し，DBに追加
        /// </summary>
        private List<Candle> GetCandle(int dataCount = 500)
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
            return result;
        }


        /// <summary>
        /// resultからテクニカル指標を生成,直近のDateを返す
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private DateTime MakeIndex(List<Candle> result)
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
  "NULL" + "," +
  "NULL" + "," +
  "NULL" + "," +
  "NULL" + "," +
  "NULL" + "," +
  "NULL" + "," +
  "NULL" + "," +
  "NULL" + "," +
"'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
  ")";
            DB.ExecuteNonQuery(sql);

            return dateStock.Last();
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
