using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;
using MySql.Data;
using static VCAPI.Repository.MySQL.DatabaseConnector;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace VCAPI.Repository.MySQL
{
    public class MySQLUserRepository : IUserRepository
    {
        private struct HashSettings{
            // Hard-coded hash.. for now.
            public const string salt = "oihergoi23r092ugwe";
            public const int hashSize = 512;
            public const int iterations = 100000;
        }

        
        readonly byte[] bsalt;
        readonly DatabaseConnector connector;
        public MySQLUserRepository(DatabaseConnector conn)
        {
            bsalt = Encoding.UTF32.GetBytes(HashSettings.salt);
            connector = conn;
        }

        public async Task<bool> Authenticate(string username, string password)
        {
           UserInfo info = await GetUser(username);
           byte[] hash = (KeyDerivation.Pbkdf2(password, bsalt, KeyDerivationPrf.HMACSHA512, HashSettings.iterations, HashSettings.hashSize));
            
            return (hash == info.password);
        }

        public async Task<UserInfo> GetUser(string username)
        {
            using(Connection conn = await connector.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "getUser";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@userID", username);
                command.Parameters["@userID"].Direction = ParameterDirection.Input;
                DbDataReader reader = await command.ExecuteReaderAsync();
                if(!await reader.NextResultAsync()){
                    return null;
                }

                UserInfo info = new UserInfo();
                info.userID = reader.GetString(0);
                reader.GetBytes(1, 0, info.password, 0, 0);
                info.firstname = reader.GetString(2);
                info.lastname = reader.GetString(3);
                return info;
            }
        }

        public async Task<bool> CreateUser(UserInfo info)
        {
           using(Connection conn = await connector.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "createUser";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", info.userID);
                command.Parameters.AddWithValue("@firstnameparam", info.firstname);
                command.Parameters.AddWithValue("@lastnameparam", info.lastname);
                command.Parameters.AddWithValue("@phonenumberparam", 0);

                byte[] hash = (KeyDerivation.Pbkdf2(Encoding.UTF8.GetString(info.password), bsalt, KeyDerivationPrf.HMACSHA512, HashSettings.iterations, HashSettings.hashSize));


                command.Parameters.AddWithValue("@passwordparam", Convert.ToBase64String(hash));
                return await command.ExecuteNonQueryAsync() == 1;
           }
        }
    }
}
