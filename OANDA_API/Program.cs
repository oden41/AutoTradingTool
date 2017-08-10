using RestApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace OANDA_API
{
    class Program
    {

        static void Main(string[] args)
        {
            var db = new DataBase();
            var id = int.Parse(ConfigurationManager.AppSettings["AccountId"]);
            //var result = Rest.GetAccountDetails(8048648);
            //var result = Rest.GetInstruments(id);
            //var result = Rest.GetPositions(id);
            //var result = Rest.GetTradeList(id);
            //var result = Rest.GetCandles("USD_JPY", id,count:5000);
            //var result = Rest.GetOrderList(id);
            //var dir = new Dictionary<string, string>();
            //dir.Add("instrument", "USD_JPY");
            //dir.Add("units", "10000");
            //dir.Add("side", "sell");
            //dir.Add("type", "market");
            //Rest.PostMarketOrder(id, dir);

            var start = new DateTime(2005, 1, 1);
            var last = new DateTime(2017, 1, 1);
            while (start < last)
            {
                var result = Rest.GetCandles("USD_JPY", id, start, 5000, granularity: "M10");
                result.ForEach(data =>
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
'{data.time.ToString("yyyy-MM-dd hh:mm:ss")}',
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
                    db.ExecuteNonQuery(insertSQL);
                });
                start = result.Last().time;
            }
        }
    }
}
