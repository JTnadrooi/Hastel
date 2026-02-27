using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public static void Read(string query, Action<MySqlDataReader> onRead, params MySqlParameter[] parameters)
        {
            using MySqlCommand cmd = CreateCommand(query, parameters);
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) onRead(reader);
        }

        public static List<T> QueryList<T>(string query, Func<MySqlDataReader, T> map, params MySqlParameter[] parameters)
        {
            List<T> list = new List<T>();

            Read(query, reader =>
            {
                list.Add(map(reader));
            }, parameters);

            return list;
        }

        public static T QuerySingle<T>(string query, Func<MySqlDataReader, T> map, params MySqlParameter[] parameters)
        {
            using var reader = CreateCommand(query, parameters).ExecuteReader();

            if (reader.Read()) return map(reader);

            throw new InvalidOperationException("QuerySingle: No rows returned for query.");
        }

        public static T QuerySingleOrDefault<T>(string query, Func<MySqlDataReader, T> map, T? defaultValue, params MySqlParameter[] parameters)
        {
            using var reader = CreateCommand(query, parameters).ExecuteReader();

            if (reader.Read()) return map(reader);

            return defaultValue!;
        }

        public static T QuerySingleOrDefault<T>(string query, Func<MySqlDataReader, T> map, T? defaultValue = default)
        {
            using var reader = CreateCommand(query).ExecuteReader();

            if (reader.Read()) return map(reader);

            return defaultValue!;
        }

        public static bool TryQuerySingle<T>(string query, Func<MySqlDataReader, T> map, [NotNullWhen(true)] out T? result, params MySqlParameter[] parameters)
        {
            using var reader = CreateCommand(query, parameters).ExecuteReader();

            if (reader.Read())
            {
                result = map(reader)!;
                return true;
            }

            result = default;
            return false;
        }
    }
}
