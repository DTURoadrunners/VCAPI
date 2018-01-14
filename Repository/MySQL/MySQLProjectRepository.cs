using System;
using System.Collections.Generic;
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
                command.CommandText = "deleteProject";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@projectId", id);
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@commentParam", comment);
               await command.ExecuteNonQueryAsync();
               return true;
            }
        }

        public async Task<ProjectInfo> GetProject(int id)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "select name from projects where ID = @id;";
                command.Parameters.Add("@id", MySqlDbType.Int64).Value = id;
                command.Prepare();
                using(DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    if(!await reader.ReadAsync()){
                        return null;
                    }
                    return new ProjectInfo(id, reader.GetString(0));
                }
                
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
                await command.ExecuteNonQueryAsync();
               // TODO: Make out param that determines if any columns were actually updated
               return true;
            }
        }

         public async Task<bool> RollbackProject(int id, int revisionId, string userId, string comment)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "rollbackProject";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@revisionId", revisionId);
                command.Parameters.AddWithValue("@projectId", id);
                command.Parameters.AddWithValue("@userID", userId);
                command.Parameters.AddWithValue("@commentParam", comment);

                await command.ExecuteNonQueryAsync();

                return true;
            }
        }    
        public async Task<RevisionInfo[]> GetRevisions(int projectId)
        {
            using(Connection conn = await connection.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "select `revisionNumber`, `userID`, `type`, `comment`, `timestamp` from projectRevisions where `projectId` = @projectId;";
                command.Parameters.AddWithValue("@projectId", projectId);
                command.Prepare();
                using(DbDataReader reader =  await command.ExecuteReaderAsync())
                {
                    List<RevisionInfo> revisions = new List<RevisionInfo>();
                    while(await reader.ReadAsync())
                    {
                        revisions.Add(new RevisionInfo(){
                            revisonId = reader.GetInt32(0),
                            author = reader.GetString(1),
                            eventType = reader.GetString(2),
                            comment = reader.GetString(3),
                            timestamp = reader.GetInt32(4)
                        });
                    }
                    return revisions.ToArray();
                }
            }
        }

     

    }
}