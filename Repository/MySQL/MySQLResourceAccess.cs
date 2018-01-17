using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using VCAPI.Repository.MySQL;
using static VCAPI.Repository.MySQL.DatabaseConnector;
using VCAPI.Repository.Interfaces;
using System.Data;

namespace VCAPI.Repository.MySQL
{
    // Should match the role table in MySQL
   
    public class MySQLResourceAccess : IResourceAccess
    {
        readonly DatabaseConnector connector;
        public MySQLResourceAccess(DatabaseConnector conn)
        {
            connector = conn;
        }
        /// <summary>
        /// Assigns the user a rank for the project
        /// </summary>
        /// <param name="username">The user to reward rank</param>
        /// <param name="projId">The project to gain rank in</param>
        /// <param name="rank">The rank to be given</param>
        /// <returns></returns>
        public async Task<bool> AssignRankForProject(string username, int projId, RANK rank)
        {
            using (Connection conn = await connector.Create())
            {
                bool success = true;
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "connectUserProject";
                command.Parameters.AddWithValue("@userid", username);
                command.Parameters.AddWithValue("@projectid", projId);
                command.Parameters.AddWithValue("@roleid", rank);
                command.Parameters.AddWithValue("@iserror", success);
                command.Parameters["iserror"].Direction = System.Data.ParameterDirection.Output;
                await command.ExecuteNonQueryAsync();
                return success;
            }
        }
        /// <summary>
        /// Returns the rank of the user for project.
        /// If the user is a superadmin, then projectId is irrelevant 
        /// and he will always have access
        /// </summary>
        /// <param name="username"></param>
        /// <param name="projId"></param>
        /// <returns></returns>
        public async Task<RANK> GetRankForProject(string username, int projId)
        {
            using(Connection conn = await connector.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "getUserRole";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@userID", username);
                command.Parameters.AddWithValue("@projectID", -1);
                command.Parameters.AddWithValue("@userRole", MySqlDbType.Int32).Direction = ParameterDirection.Output;
                await command.ExecuteNonQueryAsync();
                int rank = (int)command.Parameters["@userRole"].Value;
                return (RANK)rank;
            }
        }

        public async Task<bool> IsSuperAdmin(string username)
        {
           return await GetRankForProject(username, -1) == RANK.SUPERADMIN;
        }
    }
}