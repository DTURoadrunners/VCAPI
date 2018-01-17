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
    /// <summary>
    /// Handles category storage in a MySQL database through the CategoryRepository interface
    /// </summary>
    public class MySQLCategoryRepository : ICategoryRepository
    {
        readonly DatabaseConnector connection;
        public MySQLCategoryRepository(DatabaseConnector conn)
        {
            connection = conn;
        }

        /// <summary>
        /// Creates a new category object 
        /// used when creating componentTypes
        /// This class is not fully implemented and tested as it had low priority.
        /// </summary>
        /// <param name="projectId">The projectId that the category belongs to</param>
        /// <param name="model">The category definition</param>
        /// <param name="userId">The id of the user who should be marked as creator of the componentType</param>
        /// <param name="comment">The comment associated with the creation</param>
        /// <returns>The unique id of the newly created category</returns>
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

                await command.ExecuteNonQueryAsync();
                //TODO: Make this return the id returned by the procedure
                return -1;
            }
        }

        /// <summary>
        /// Gets all categories associated with a project
        /// </summary>
        /// <param name="projectId">The id of the project</param>
        /// <returns>A list of categories</returns>
        public async Task<List<CategoryInfo>> GetCategories(int projectId)
        {
            using (Connection conn = await connection.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "getCategories";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@projectId", projectId);
                DbDataReader reader = await command.ExecuteReaderAsync();
               
                List<CategoryInfo> list = new List<CategoryInfo>();
                while (await reader.ReadAsync())
                {
                    list.Add(new CategoryInfo(reader.GetInt32(0), reader.GetString(1)));
                }

                return list;
            }
        }


        /// <summary>
        /// Gets a specific category. No projectId is needed as every category has 
        /// unique id's
        /// </summary>
        /// <param name="id">The id of the category</param>
        /// <returns>The specific category requested or null if it does not exist</returns>
        public async Task<CategoryInfo> GetCategory(int id)
        {
            using (Connection conn = await connection.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "getCategory";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", id);
                DbDataReader reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return null;
                }

                return new CategoryInfo(reader.GetInt32(0), reader.GetString(1));
            }
        }
    }       
}
