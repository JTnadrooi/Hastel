using AsitLib.CommandLine;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hastel.Server.CommandProviders
{
    public class AuthCommandProvider : CommandGroup
    {
        public AuthCommandProvider() : base("auth")
        {

        }

        [Command("User login.")]
        public CommandWebResponse Login_POST(string username, string password)
        {
            // check if inputs are valid.
            if (QueryHelper.TryQuerySingle("SELECT userid, username, password FROM users WHERE username = @username", User.Map, out User? user, new MySqlParameter("@username", username))
                    && user.Password == password)
            {
                string token = user.GetToken();

                return new CommandWebResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    String = JsonConvert.SerializeObject(new { token = token }),
                    Cookies = [new Cookie("authToken", token)
                    {
                        HttpOnly = true,
                        Path = "/"
                    }]
                };
            }
            else
            {
                throw new WebException("Invalid username or password.", HttpStatusCode.Unauthorized);
            }
        }

        [Command("Register new user.")]
        public CommandWebResponse Register_POST(string username, string password)
        {
            // check if user already exists.
            if (QueryHelper.TryQuerySingle("SELECT userid FROM users WHERE username = @username",
                reader => reader.GetInt32("userid"), out _, new MySqlParameter("@username", username)))
            {
                return CommandWebResponse.FromStatusCode(HttpStatusCode.Conflict, "Username already taken");
            }

            string insertQuery = "INSERT INTO users (username, password) VALUES (@username, @password); SELECT LAST_INSERT_ID();";

            int newUserId = QueryHelper.ExecuteScalar<int>(insertQuery,
                new MySqlParameter("@username", username),
                new MySqlParameter("@password", password));

            if (newUserId > 0) // if its less, query did nothing, so it failed.
            {
                return CommandWebResponse.Succes;
            }
            else
            {
                throw new WebException("Failed to create user");
            }
        }

        [Command("Change user password.")]
        public CommandWebResponse ChangePassword_POST(string username, string newPassword)
        {
            ThrowHelper.ThrowIfInvalidLogin(username);

            // check if user exists.
            if (!QueryHelper.TryQuerySingle("SELECT username FROM users WHERE username = @username",
                reader => reader.GetString("username"), out _, new MySqlParameter("@username", username)))
            {
                return CommandWebResponse.FromStatusCode(HttpStatusCode.NotFound, "User not found");
            }

            string updateQuery = "UPDATE users SET password = @password WHERE username = @username";

            using MySqlCommand cmd = new MySqlCommand(updateQuery, QueryHelper.Connection);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", newPassword);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0) // if its less, query didnt find user or something else failed.
            {
                return CommandWebResponse.Succes;
            }
            else
            {
                throw new WebException("Failed to update password");
            }
        }

        [Command("Delete user.")]
        public CommandWebResponse DeleteUser_POST(string username)
        {
            ThrowHelper.ThrowIfInvalidLogin(username);

            // check if user exists.
            if (!QueryHelper.TryQuerySingle("SELECT username FROM users WHERE username = @username",
                reader => reader.GetString("username"), out _, new MySqlParameter("@username", username)))
            {
                return CommandWebResponse.FromStatusCode(HttpStatusCode.NotFound, "User not found");
            }

            string deleteQuery = "DELETE FROM users WHERE username = @username";

            using MySqlCommand cmd = new MySqlCommand(deleteQuery, QueryHelper.Connection);
            cmd.Parameters.AddWithValue("@username", username);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0) // if its less, query didnt find user.
            {
                return CommandWebResponse.Succes;
            }
            else
            {
                throw new WebException("Failed to delete user");
            }
        }
    }
}
