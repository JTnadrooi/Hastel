using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hastel.Server
{
    public record User(int UserId, string Username, string Password)
    {
        /// <summary>
        /// Converts this user to a token. Security pending.
        /// </summary>
        public string GetToken()
        {
            return (Username + Password).GetHashCode().ToString() + "id" + UserId;
        }

        /// <summary>
        /// Maps a user from a db column as supplied by the specified <paramref name="reader"/>.
        /// </summary>
        public static User Map(MySqlDataReader reader)
            => new User(reader.GetInt32("userid"), reader.GetString("username"), reader.GetString("password"));

        /// <summary>
        /// Creates a valid <see cref="User"/> instance from a token.
        /// </summary>
        public static User? FromToken(string token)
        {
            int userId = int.Parse(token.Split("id").Last());

            if (QueryHelper.TryQuerySingle(
               "SELECT userid, username, password FROM users WHERE userid = @id",
               User.Map, out User? user,
               new MySqlParameter("@id", userId)
            ))
            {
                return user;
            }
            else return null;
        }
    }
}
