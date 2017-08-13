using RestApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace OANDA_API
{
    class DataMaker
    {
        public static void GetDataFromOANDA(int id, string ccyPair)
        {
            var db = new DataBase();
            var start = new DateTime(2017, 2, 8, 2, 40, 0);
            var last = new DateTime(2017, 8, 10, 0, 0, 0);
            while (start < last)
            {
                var result = Rest.GetCandles(ccyPair, id, start, 5000, granularity: "M10");
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
                    db.ExecuteNonQuery(insertSQL);
                });
                start = result.Last().time;
            }
        }
    }
}
