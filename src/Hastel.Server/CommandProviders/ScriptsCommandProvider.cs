using AsitLib.CommandLine;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Hastel.Server.CommandProviders
{
    public class ScriptsCommandProvider : CommandGroup
    {
        public ScriptsCommandProvider() : base("scripts")
        {

        }

        [Command(".")]
        public CommandWebResponse SaveScript_POST(string username, string text, string name)
        {
            ThrowHelper.ThrowIfInvalidLogin(username);

            string checkQuery = "SELECT COUNT(*) FROM scripts WHERE owner_id = @owner_id AND name = @name";
            int existingCount = QueryHelper.ExecuteScalar<int>(checkQuery,
                new MySqlParameter("@owner_id", Program.CurrentUser!.UserId),
                new MySqlParameter("@name", name));

            string insertQuery;
            MySqlParameter[] parameters;

            if (existingCount > 0)
            {
                string updateQuery = "UPDATE scripts SET script_text = @script_text WHERE owner_id = @owner_id AND name = @name";
                int scriptId = QueryHelper.ExecuteScalar<int>(updateQuery,
                    new MySqlParameter("@script_text", text),
                    new MySqlParameter("@owner_id", Program.CurrentUser!.UserId),
                    new MySqlParameter("@name", name));
            }
            else
            {
                insertQuery = "INSERT INTO scripts (owner_id, name, script_text) VALUES (@owner_id, @name, @script_text)";
                parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@owner_id", Program.CurrentUser!.UserId),
                    new MySqlParameter("@name", name),
                    new MySqlParameter("@script_text", text)
                };

                int scriptId = QueryHelper.ExecuteScalar<int>(insertQuery, parameters);
            }

            return CommandWebResponse.Succes;
        }

        [Command(".")]
        public CommandWebResponse LoadScript_POST(string name)
        {
            string checkQuery = "SELECT id, name, script_text FROM scripts WHERE owner_id = @owner_id AND name = @name";
            bool existingScript = QueryHelper.TryQuerySingle(checkQuery, reader => new
            {
                id = reader.GetInt32("id"),
                name = reader.GetString("name"),
                text = reader.GetString("script_text")
            }, out var script, new MySqlParameter("@owner_id", Program.CurrentUser!.UserId), new MySqlParameter("@name", name));

            if (existingScript)
            {
                string json = JsonConvert.SerializeObject(script);
                return CommandWebResponse.FromString(json);
            }
            else
            {
                return CommandWebResponse.FromStatusCode(HttpStatusCode.NotFound, "Script not found");
            }
        }

        [Command(".")]
        public CommandWebResponse List_POST(string username)
        {
            ThrowHelper.ThrowIfInvalidLogin(username);

            string query = "SELECT name FROM scripts WHERE owner_id = @owner_id ORDER BY name";

            List<string> scriptNames = QueryHelper.QueryList(query,
                reader => reader.GetString("name"),
                new MySqlParameter("@owner_id", Program.CurrentUser!.UserId));

            string json = JsonConvert.SerializeObject(scriptNames);

            return CommandWebResponse.FromString(json);
        }

        [Command(".")]
        public void DeleteScript_POST(string name)
        {
            string checkQuery = "SELECT COUNT(*) FROM scripts WHERE owner_id = @owner_id AND name = @name";
            int existingCount = QueryHelper.ExecuteScalar<int>(checkQuery,
                new MySqlParameter("@owner_id", Program.CurrentUser!.UserId),
                new MySqlParameter("@name", name));

            if (existingCount == 0)
            {
                throw new WebException("Script not found", HttpStatusCode.NotFound);
            }

            string deleteQuery = "DELETE FROM scripts WHERE owner_id = @owner_id AND name = @name";
            QueryHelper.ExecuteScalar<int>(deleteQuery,
                new MySqlParameter("@owner_id", Program.CurrentUser!.UserId),
                new MySqlParameter("@name", name));
        }
    }
}
