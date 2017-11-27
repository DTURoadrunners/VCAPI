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

namespace VCAPI.Repository.MySQL
{
    public class MySQLCategoryRepository : ICategoryRepository
    {
        DatabaseConnector connection;
        public MySQLCategoryRepository(DatabaseConnector conn)
        {
            connection = conn;
        }

        public async Task<bool> CreateCategory(string name)
        {
            using(Connection conn = await connection.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "createCategory";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@nameparam", name);

                return await command.ExecuteNonQueryAsync() == 1;          
            }
        }
    }       
}
