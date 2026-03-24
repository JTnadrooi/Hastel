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
        /// <summary>
        /// Gets or sets the DB connection.
        /// </summary>
        public static MySqlConnection Connection { get; set; }
#nullable enable

        // helper
        private static MySqlCommand CreateCommand(string query, params MySqlParameter[] parameters)
        {
            MySqlCommand cmd = new MySqlCommand(query, Connection);

            if (parameters is not null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            return cmd;
        }

        /// <summary>
        /// Executes a query and returns the first column of the first row, cast to type T.
        /// </summary>
        public static T ExecuteScalar<T>(string query, params MySqlParameter[] parameters)
        {
            using MySqlCommand cmd = CreateCommand(query, parameters);
            object? result = cmd.ExecuteScalar();

            if (result is null || result == DBNull.Value)
                return default!;

            return (T)Convert.ChangeType(result, typeof(T));
        }

        /// <summary>
        /// Executes a query and invokes the callback for each row read.
        /// </summary>
        public static void Read(string query, Action<MySqlDataReader> onRead, params MySqlParameter[] parameters)
        {
            using MySqlCommand cmd = CreateCommand(query, parameters);
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) onRead(reader);
        }

        /// <summary>
        /// Gets a list of mapped results from the query.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of mapped objects.</returns>
        public static List<T> QueryList<T>(string query, Func<MySqlDataReader, T> map, params MySqlParameter[] parameters)
        {
            List<T> list = new List<T>();

            Read(query, reader =>
            {
                list.Add(map(reader));
            }, parameters);

            return list;
        }

        /// <summary>
        /// Gets a single mapped result.
        /// </summary>
        /// <returns>The mapped object.</returns>
        /// <remarks>Throws <see cref="InvalidOperationException"/> if no rows are returned.</remarks>
        public static T QuerySingle<T>(string query, Func<MySqlDataReader, T> map, params MySqlParameter[] parameters)
        {
            using MySqlDataReader reader = CreateCommand(query, parameters).ExecuteReader();

            if (reader.Read()) return map(reader);

            throw new InvalidOperationException("QuerySingle: No rows returned for query.");
        }

        /// <summary>
        /// Gets a single mapped result or the default value if no rows are returned.
        /// </summary>
        /// <returns>The mapped object or <paramref name="defaultValue"/>.</returns>
        public static T QuerySingleOrDefault<T>(string query, Func<MySqlDataReader, T> map, T? defaultValue, params MySqlParameter[] parameters)
        {
            using MySqlDataReader reader = CreateCommand(query, parameters).ExecuteReader();

            if (reader.Read()) return map(reader);

            return defaultValue!;
        }

        /// <summary>
        /// Gets a single mapped result or the default value if no rows are returned.
        /// </summary>
        /// <returns>The mapped object or <see langword="default"/>.</returns>
        public static T QuerySingleOrDefault<T>(string query, Func<MySqlDataReader, T> map, T? defaultValue = default)
        {
            using MySqlDataReader reader = CreateCommand(query).ExecuteReader();

            if (reader.Read()) return map(reader);

            return defaultValue!;
        }

        /// <summary>
        /// Tries to get a single mapped result.
        /// </summary>
        /// <returns><see langword="true"/> if a row was read; otherwise, <see langword="false"/>.</returns>
        public static bool TryQuerySingle<T>(string query, Func<MySqlDataReader, T> map, [NotNullWhen(true)] out T? result, params MySqlParameter[] parameters)
        {
            using MySqlDataReader reader = CreateCommand(query, parameters).ExecuteReader();

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