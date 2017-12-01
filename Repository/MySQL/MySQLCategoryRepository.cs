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

        public async Task<int> CreateCategory(int projectId, CategoryInfo model, string userId, string comment)
        {
            using(Connection conn = await connection.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "createCategory";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@projectId", projectId);
                command.Parameters.AddWithValue("@nameparam", model.name);
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@commentparam", comment);

                return await command.ExecuteNonQueryAsync();          
            }
        }

        public async Task<bool> DeleteCategory(int projectId, int id, string userId, string comment)
        {
            using (Connection conn = await connection.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "deleteCategory";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@projectId", projectId);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@commentparam", comment);

                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<List<CategoryInfo>> GetCategories(int projectId)
        {
            using (Connection conn = await connection.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "getCategories";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@projectId", projectId);
                DbDataReader reader = await command.ExecuteReaderAsync();
                if (!await reader.NextResultAsync())
                {
                    return null;
                }
                List<CategoryInfo> list = new List<CategoryInfo>();
                while (reader.NextResult())
                {
                    list.Add(new CategoryInfo(reader.GetInt32(0), reader.GetString(1)));
                }

                return list;
            }
        }

        public async Task<CategoryInfo> GetCategory(int id)
        {
            using (Connection conn = await connection.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "getCategory";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", id);
                DbDataReader reader = await command.ExecuteReaderAsync();
                if (!await reader.NextResultAsync())
                {
                    return null;
                }

                return new CategoryInfo(reader.GetInt32(0), reader.GetString(1));
            }
        }

        public async Task<bool> UpdateCategory(int projectId, CategoryInfo model, string userId, string comment)
        {
            using (Connection conn = await connection.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "updateCategory";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@projectId", projectId);
                command.Parameters.AddWithValue("@id", model.id);
                command.Parameters.AddWithValue("@name", model.name);
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@commentparam", comment);

                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }       
}
