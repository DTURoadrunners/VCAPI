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
    public class MySQLProjectRepository : IProjectRepository
    {
        readonly DatabaseConnector connection;
        public MySQLProjectRepository(DatabaseConnector conn)
        {
            connection = conn;
        }

        public async Task<int> CreateProject(string name, string userId, string comment)
        {
            using(Connection conn = await connection.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "createProject";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@nameparam", name);
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@commentparam", comment);
                command.Parameters.Add("@id", DbType.Int32).Direction = ParameterDirection.Output;
                command.ExecuteNonQuery();
                return (int)command.Parameters["@id"].Value;
            }
        }

        public async Task<bool> DeleteProject(int id, string userId, string comment)
        {
             using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "deleteDocument";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@activeProjectID", id);
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@commentparam", comment);
               
               return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<ProjectInfo> GetProject(int id)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "getProject";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@projectID", id);
                command.Parameters["@projectID"].Direction = ParameterDirection.Input;
                DbDataReader reader = await command.ExecuteReaderAsync();
                if(!await reader.NextResultAsync()){
                    return null;
                }
                
                return new ProjectInfo(reader.GetInt32(0), reader.GetString(1));
            }
        }

        public async Task<List<ProjectInfo>> GetProjects()
        {
            try{
                using(Connection conn = await connection.Create()){
                                MySqlCommand command = conn.Get().CreateCommand();
                                command.CommandText = "getActiveProjects";
                                command.CommandType = CommandType.StoredProcedure;
                                DbDataReader reader = await command.ExecuteReaderAsync();
                                if(!await reader.NextResultAsync()){
                                    return null;
                                }
                                List <ProjectInfo> list = new List<ProjectInfo>();
                                while (reader.NextResult()){
                                    list.Add(new ProjectInfo(reader.GetInt32(0), reader.GetString(1)));
                                }
                                
                                return list;
                            }
            }catch(MySqlException e)
            {
               return new List<ProjectInfo>();
            }
           
        }

        public async Task<bool> UpdateProject(ProjectInfo inf, int id, string userId, string comment)
        {
             using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "updateProject";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@nameparam", inf.name);
                command.Parameters.AddWithValue("@activeProjectID", id);
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@commentparam", comment);
               // TODO: Make out param that determines if any columns were actually updated
               return true;
            }
        }

         public async Task<bool> RollbackProject(int id, string userId, string comment)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "rollbackProject";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@logID", id);
                command.Parameters.AddWithValue("@userID", userId);
                command.Parameters.AddWithValue("@commentParam", comment);
               
               return await command.ExecuteNonQueryAsync() == 1;
            }
        }

    }
}