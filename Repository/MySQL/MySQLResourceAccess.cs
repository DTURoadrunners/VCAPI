using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using VCAPI.Repository.MySQL;
using static VCAPI.Repository.MySQL.DatabaseConnector;

namespace VCAPI.Repository
{
    public enum RANK
    {
        PROHIBITED,
        GUEST,
        STUDENT,
        ADMIN,
        SUPERADMIN
    }
    public class MySQLResourceAccess
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
    }
}