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
    public class MySQLComponentTypeRepository : IComponentTypeRepository
    {
        readonly DatabaseConnector connection;
        public MySQLComponentTypeRepository(DatabaseConnector conn)
        {
            connection = conn;
        }

        /// <summary>
        /// Creates a new component type
        /// </summary>
        /// <param name="info">The type to create</param>
        /// <param name="projectId">The projectId to associate with the componentype</param>
        /// <param name="userId">The creator of the componenttype</param>
        /// <param name="comment">The reason behind the creation</param>
        /// <returns>The ID of the newly created component type</returns>
        public async Task<int> CreateComponentType(ComponentTypeInfo info, int projectId , string userId, string comment)
        {
            using(Connection conn = await connection.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "createComponenttype";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@nameparam", info.name);
                command.Parameters.AddWithValue("@activeProjectIDParam", projectId);
                command.Parameters.AddWithValue("@categoryId", info.categoryID);
                command.Parameters.AddWithValue("@storageparam", info.storage);
                command.Parameters.AddWithValue("@description", info.description);
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@commentparam", comment);
                command.Parameters.Add("@id", DbType.Int32).Direction = ParameterDirection.Output;
                await command.ExecuteNonQueryAsync();
                return (int)command.Parameters["@id"].Value;
            }
        }

        /// <summary>
        /// Gets a component type
        /// </summary>
        /// <param name="componentTypeId">The Id of the component type to get</param>
        /// <param name="projectId">The project which the component type belongs to</param>
        /// <returns>The component type</returns>
         public async Task<ComponentTypeInfo> GetComponentType(int componentTypeId, int projectId)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "select ID, name, categoryID, storage, description from componentTypes where ID = @ID AND associatedProject = @projectId;";
                command.Parameters.Add("@ID", DbType.Int32).Value = componentTypeId;
                command.Parameters.Add("@projectId", DbType.Int32).Value = projectId;
                command.Prepare();
                DbDataReader reader = await command.ExecuteReaderAsync();
                if(!await reader.ReadAsync()){
                    return null;
                }
                
                return new ComponentTypeInfo(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetString(4));
            }
        }

        /// <summary>
        /// Gets all component types for a project
        /// </summary>
        /// <param name="projectId">The project to get component types from</param>
        /// <returns>A list of component types</returns>
        public async Task<List<ComponentTypeInfo>> GetComponentTypes(int projectId)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "select ID, name, categoryID, storage, description from componentTypes where associatedProject = @projectId";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@projectId", projectId);
                command.Prepare();
                DbDataReader reader = await command.ExecuteReaderAsync();
                List <ComponentTypeInfo> list = new List<ComponentTypeInfo>();
                while (await reader.ReadAsync()){
                    list.Add(new ComponentTypeInfo(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetString(4)));
                }
                return list;
            }
        }
        
        /// <summary>
        /// Updates a component type
        /// </summary>
        /// <param name="info">The component type to update(including the id of the component type to update)</param>
        /// <param name="userId">The id of the user performing the update</param>
        /// <param name="comment">The reason behind the update</param>
        /// <returns></returns>
        public async Task<bool> UpdateComponentType(ComponentTypeInfo info, string userId, string comment)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "updateComponenttype";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@nameparam", info.name);
                command.Parameters.AddWithValue("@componentTypeId", info.id);
                command.Parameters.AddWithValue("@categoryId", info.categoryID);
                command.Parameters.AddWithValue("@storageparam", info.storage);
                command.Parameters.AddWithValue("@descriptionparam", info.description);
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@commentparam", comment);
                await command.ExecuteNonQueryAsync();
                return true;
            }
        }

        /// <summary>
        /// Deletes a component type
        /// </summary>
        /// <param name="componentTypeId">The componentType to delete</param>
        /// <param name="userId">The user to be logged as the one performing the deletion</param>
        /// <param name="comment">The reason of the deletion</param>
        /// <returns></returns>
        public async Task<bool> DeleteComponentType(int componentTypeId, string userId, string comment)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "deleteComponenttype";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@activeComponentTypeID", componentTypeId);
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@commentparam", comment);
                await command.ExecuteNonQueryAsync();
                return true;
            }
        }
        
        /// <summary>
        /// Gets the change history for a component type
        /// </summary>
        /// <param name="componentTypeId">The component type to get the changeset history from</param>
        /// <returns>A list of changesets</returns>
        public async Task<RevisionInfo[]> GetRevisionAsync(int componentTypeId)
        {
            using(Connection conn = await connection.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "select `revision`, `userID`, `type`, `comment`, `timestamp` from componentTypeRevision where `componentTypeID` = @componentType;";
                command.Parameters.AddWithValue("@componentType", componentTypeId);
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


        /// <summary>
        /// Rolls back the component type to an earlier revision.
        /// The id of the type to update is gained from the revisionId
        /// </summary>
        /// <param name="revisionId">The revision to rollback to</param>
        /// <param name="userId">The id of the user performing the rollback</param>
        /// <param name="comment">Reason to be logged for the rollback</param>
        /// <returns>Always true</returns>
        public async Task<bool> RollbackComponentType(int revisionId, string userId, string comment)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "rollbackComponenttype";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@revisionId", revisionId);
                command.Parameters.AddWithValue("@userID", userId);
                command.Parameters.AddWithValue("@commentParam", comment);
               
                await command.ExecuteNonQueryAsync();
                return true;
            }        
        }
    }       
}
