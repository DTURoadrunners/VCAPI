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
        public async Task<bool> CreateProject(LogInfo log, string name)
        {
            using(Connection conn = await connection.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "createProject";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@nameparam", name);
                command.Parameters.AddWithValue("@userid", name);
                command.Parameters.AddWithValue("@commentparam", name);
                
                return await command.ExecuteNonQueryAsync() == 1;          
            }
        }

        public async Task<bool> DeleteProject(int id, LogInfo log)
        {
             using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "deleteDocument";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@activeProjectID", id);
                command.Parameters.AddWithValue("@userid", log.userID);
                command.Parameters.AddWithValue("@commentparam", log.comment);
               
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

                ProjectInfo info = new ProjectInfo();
                info.id = reader.GetInt32(0);
                info.name = reader.GetString(1);
                return info;
            }
        }

        public async Task<List<ProjectInfo>> GetProjects()
        {
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
                    ProjectInfo info = new ProjectInfo();
                    info.id = reader.GetInt32(0);
                    info.name = reader.GetString(1);
                    list.Add(info);
                }
                
                return list;
            }
        }

        public async Task<bool> UpdateProject(ProjectInfo inf, int id, LogInfo log)
        {
             using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "updateProject";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@nameparam", inf.name);
                command.Parameters.AddWithValue("@activeProjectID", log.activeID);
                command.Parameters.AddWithValue("@userid", log.userID);
                command.Parameters.AddWithValue("@commentparam", log.comment);
               
               return await command.ExecuteNonQueryAsync() == 1;
            }
        }

         public async Task<bool> RollbackProject(int id, LogInfo log)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "rollbackProject";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@logID", id);
                command.Parameters.AddWithValue("@userID", log.userID);
                command.Parameters.AddWithValue("@commentParam", log.comment);
               
               return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}