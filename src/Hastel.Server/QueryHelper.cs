using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hastel.Server
{
    public static class QueryHelper
    {
#nullable disable
        public static MySqlConnection Connection { get; set; }
#nullable enable

        private static MySqlCommand CreateCommand(string query, params MySqlParameter[] parameters)
        {
            MySqlCommand cmd = new MySqlCommand(query, Connection);

            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            return cmd;
        }

        public static void Execute(string query, Action<MySqlDataReader> onRead, params MySqlParameter[] parameters)
        {
            using MySqlCommand cmd = CreateCommand(query, parameters);
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) onRead(reader);
        }

        public static List<T> QueryList<T>(string query, Func<MySqlDataReader, T> map, params MySqlParameter[] parameters)
        {
            List<T> list = new List<T>();

            Execute(query, reader =>
            {
                list.Add(map(reader));
            }, parameters);

            return list;
        }
    }
}
