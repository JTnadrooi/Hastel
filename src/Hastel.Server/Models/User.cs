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
        public string GetToken()
        {
            return (Username + Password).GetHashCode().ToString() + "id" + UserId;
        }

        public static User Map(MySqlDataReader reader)
            => new User(reader.GetInt32("userid"), reader.GetString("username"), reader.GetString("password"));

        public static User FromToken(string token)
        {
            return new User(int.Parse(token.Split("id").Last()), string.Empty, string.Empty);
        }
    }
}
