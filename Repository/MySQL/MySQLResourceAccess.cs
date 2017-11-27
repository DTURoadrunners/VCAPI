using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using VCAPI.Repository.MySQL;
using static VCAPI.Repository.MySQL.DatabaseConnector;

namespace VCAPI.Repository
{
    // Should match the role table in MySQL
    public enum RANK
    {
        PROHIBITED = 1,
        GUEST = 2,
        STUDENT = 3,
        ADMIN = 4,
        SUPERADMIN = 5
    }
    public class MySQLResourceAccess : Interfaces.IResourceAccess
    {
        readonly DatabaseConnector connector;
        public MySQLResourceAccess(DatabaseConnector conn)
        {
            connector = conn;
        }

        public async Task<RANK> GetRankForProject(string username, int projId)
        {
            using(Connection conn = await connector.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "getUserRole";
                command.Parameters.AddWithValue("@userID", username);
                command.Parameters.AddWithValue("@projectID", projId);
                DbDataReader reader = await command.ExecuteReaderAsync();
                if(!await reader.NextResultAsync()){
                    return RANK.PROHIBITED;
                }
                else{
                    int rank = reader.GetInt32(0);
                    return (RANK)rank;
                }

            }
        }

        public async Task<bool> IsSuperAdmin(string username)
        {
           using(Connection conn = await connector.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "getUserRole";
                command.Parameters.AddWithValue("@userID", username);
                command.Parameters.AddWithValue("@projectID", -1);
                DbDataReader reader = await command.ExecuteReaderAsync();
                if(!await reader.NextResultAsync()){
                    return false;
                }
                else{
                    int rank = reader.GetInt32(0);
                    return (RANK)rank == RANK.SUPERADMIN;
                }
            }
        }
    }
}