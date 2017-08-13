using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class DataBase
    {
        public string ConnString
        {
            get;set;
        }

        public DataBase()
        {
            this.ConnString = ConfigurationManager.AppSettings["ConnString"];
        }


        /// <summary>
        /// SQLにより発行したテーブルの1行1列目を返すメソッド
        /// </summary>
        /// <param name="SQL">SQL文</param>
        /// <param name="def">エラー時に返す値</param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string SQL, T def)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                return ExecuteScalar(conn, SQL, def);
            }
        }


        public T ExecuteScalar<T>(SqlConnection conn, string SQL, T def)
        {
            // SQLを実行します。
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                var command = new SqlCommand(SQL, conn);
                command.Connection = conn;
                return (T)Convert.ChangeType(command.ExecuteScalar(), typeof(T));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine($"SQL:{SQL}");
                return def;
            }
        }

        /// <summary>
        /// SQL(SELECT)により得られたDataTableを返すメソッド
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public DataTable ExecuteReader(string SQL)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(ConnString))
            {
                return ExecuteReader(conn, SQL);
            }
        }


        public DataTable ExecuteReader(SqlConnection conn, string SQL)
        {
            try
            {
                var dt = new DataTable();
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                var command = new SqlCommand(SQL, conn);
                command.Connection = conn;
                command.CommandTimeout = 30;
                // SQLを実行します。
                var reader = command.ExecuteReader();
                // 型付テーブルを作成します。
                dt = CreateSchemaDataTable(reader);
                while (reader.Read())
                {
                    var row = dt.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[i] = reader.GetValue(i);
                    }
                    dt.Rows.Add(row);
                }
                return dt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine($"SQL:{SQL}");
                return null;
            }
        }


        /// <summary>
        /// INSERTやUPDATEなど，DBに書き込むためのメソッド
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns>成功:true,失敗:false</returns>
        public bool ExecuteNonQuery(string SQL)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                conn.Open();

                SqlCommand command = new SqlCommand(SQL, conn);
                command.Connection = conn;
                // SQLを実行します。
                try
                {
                    command.ExecuteNonQuery();
                    return true;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine($"SQL:{SQL}");
                    return false;
                }
            }
        }


        /// <summary>
        /// SqlDataReaderで取得した構造を元にDataTableを作成します。
        /// </summary>
        /// <param name="reader">SqlDataReaderオブジェクト</param>
        /// <returns>DataTableオブジェクト</returns>
        /// <remarks>
        /// 参考：
        /// http://msdn.microsoft.com/ja-jp/library/system.data.datatablereader.getschematable.aspx
        /// </remarks>
        private DataTable CreateSchemaDataTable(SqlDataReader reader)
        {
            if (reader == null) { return null; }
            if (reader.IsClosed) { return null; }

            DataTable schema = reader.GetSchemaTable();
            DataTable dt = new DataTable();

            foreach (DataRow row in schema.Rows)
            {
                // Column情報を追加してください。
                DataColumn col = new DataColumn();
                col.ColumnName = row["ColumnName"].ToString();
                col.DataType = Type.GetType(row["DataType"].ToString());

                if (col.DataType.Equals(typeof(string)))
                {
                    col.MaxLength = (int)row["ColumnSize"];
                }

                dt.Columns.Add(col);
            }
            return dt;
        }
    }
}
