using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ServerWorker
{
    class MySQLManager
    {
        public static void Send(MySQLData data)
        {
            string columnValue = "";
            string dataValue = "";
            int columnsCount = data.Columns.Length;

            for (int i = 0; i < columnsCount; i++)
            {
                columnValue += "`" + data.Columns[i] + "`";
                if (i + 1 < columnsCount) columnValue += ",";
            }

            foreach (string[] row in data.Values)
            {
                dataValue += "(";
                foreach (string cell in row)
                {
                    dataValue += "'" + cell + "',";
                }
                dataValue = dataValue.Remove(dataValue.Length-1);
                dataValue += "),";
            }
            dataValue = dataValue.Remove(dataValue.Length - 1);

            string connStr = "server=fizerfoxdb.cf8gvhcbowrr.us-east-1.rds.amazonaws.com;user=Fizerfox;database=Fizerfox;password=EytG4GpR34DLc7N;charset=utf8;";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string sql_registrUser = String.Format("INSERT INTO `Fizerfox`.`{0}` ({1}) VALUES {2};",
                data.Table,
                columnValue,
                dataValue
                );

            MySqlCommand comm_registr = new MySqlCommand(sql_registrUser, conn);
            comm_registr.ExecuteNonQuery();
            conn.Close();
        }
    }

    public class MySQLData
    {
        public string Table;
        public string[] Columns;
        public List<string[]> Values = new List<string[]>();
    }
}
