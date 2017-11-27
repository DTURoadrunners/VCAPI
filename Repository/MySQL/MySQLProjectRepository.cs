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
        public async Task<bool> CreateProject(string userid, string comment, string name)
        {
            using(Connection conn = await connection.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "createProject";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@nameparam", name);
                command.Parameters.AddWithValue("@userid", userid);
                command.Parameters.AddWithValue("@commentparam", comment);
                
                return await command.ExecuteNonQueryAsync() == 1;          
            }
        }

        public async Task<bool> DeleteProject(int id, string userid, string comment)
        {
             using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "deleteDocument";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@activeProjectID", id);
                command.Parameters.AddWithValue("@userid", userid);
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

        public async Task<bool> UpdateProject(ProjectInfo inf, int id, string userid, string comment)
        {
             using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "updateProject";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@nameparam", inf.name);
                command.Parameters.AddWithValue("@activeProjectID", id);
                command.Parameters.AddWithValue("@userid", userid);
                command.Parameters.AddWithValue("@commentparam", comment);
               
               return await command.ExecuteNonQueryAsync() == 1;
            }
        }

         public async Task<bool> RollbackProject(int id, string userid, string comment)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "rollbackProject";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@logID", id);
                command.Parameters.AddWithValue("@userID", userid);
                command.Parameters.AddWithValue("@commentParam", comment);
               
               return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}