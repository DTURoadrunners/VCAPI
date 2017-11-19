using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;
using static VCAPI.Repository.MySQL.DatabaseConnector;

namespace VCAPI.Repository.MySQL
{
    public class MySQLUserRepository : IUserRepository
    {
        DatabaseConnector connection;
        public MySQLUserRepository(DatabaseConnector conn)
        {
            connection = conn;
        }

        public async Task<UserInfo> CreateUser(string username, string password)
        {
            using(Connection connector = await connection.Create())
            {
                return new UserInfo { name = connector.Get().Database };          
            }
        }
    }
}
