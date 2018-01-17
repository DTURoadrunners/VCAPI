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
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace VCAPI.Repository.MySQL
{
    /// <summary>
    /// Handles component storage in a MySQL database through the ComponentRepository interface
    /// </summary>
    public class MySQLComponentRepository : IComponentRepository
    {
        readonly DatabaseConnector connector;
        public MySQLComponentRepository(DatabaseConnector conn)
        {
            connector = conn;
        }

        /// <summary>
        /// Creates a component and associates it with a componentType
        /// </summary>
        /// <param name="componentTypeID">The component type to be associated with</param>
        /// <param name="component">The component information</param>
        /// <param name="userid">The Id of the user who should be marked as created the component</param>
        /// <param name="comment">The comment for the creation</param>
        /// <returns>The id of the newly created component</returns>
        public async Task<int> CreateComponent(int componentTypeID, ComponentInfo component, string userid, string comment)
        {
            using (Connection conn = await connector.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "createComponent";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@componentTypeId", componentTypeID);
                command.Parameters.AddWithValue("@status", component.status);
                command.Parameters.AddWithValue("@comment", component.comment);
                command.Parameters.AddWithValue("@userID", userid);
                command.Parameters.AddWithValue("@logComment", comment);
                command.Parameters.Add("@id", DbType.Int32).Direction = ParameterDirection.Output;
                await command.ExecuteNonQueryAsync();

                return (int)command.Parameters["@id"].Value;
            }
        }
        /// <summary>
        /// Deletes a component
        /// </summary>
        /// <param name="componentId">The id of the component</param>
        /// <param name="userid">The id of the user to be marked as the one who deleted the component</param>
        /// <param name="comment">The reason for deletion</param>
        /// <returns></returns>
        public async Task<bool> DeleteComponent(int componentId, string userid, string comment)
        {
            using (Connection conn = await connector.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "deleteComponent";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", componentId);
                command.Parameters.AddWithValue("@userID", userid);
                command.Parameters.AddWithValue("@logComment", comment);
                await command.ExecuteNonQueryAsync();
                return true;
            }
        }

        /// <summary>
        /// Gets a component from its Id
        /// </summary>
        /// <param name="componentId">The id of the component to get</param>
        /// <returns>The component or null if it does not exist</returns>
        public async Task<ComponentInfo> GetComponent(int componentId)
        {
            using (Connection conn = await connector.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "select ID, typeID, status, comment as description from components where ID = @ID;";
                command.Parameters.Add("@ID", DbType.Int32).Value = componentId;
                command.Prepare();
                DbDataReader reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return null;
                }
                return new ComponentInfo(reader.GetInt32(0), reader.GetString(2), reader.GetString(3));
            }
        }

        /// <summary>
        /// Gets a list of components from their corresponding type
        /// </summary>
        /// <param name="componentTypeId">The id of the type of component</param>
        /// <returns>A list of components all with the same type</returns>
        public async Task<List<ComponentInfo>> GetComponents(int componentTypeId)
        {
            using (Connection conn = await connector.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "select ID, status, command as description from components where typeID = @ID;";
                command.Parameters.Add("@ID", DbType.Int32).Value = componentTypeId;
                command.Prepare();
                DbDataReader reader = await command.ExecuteReaderAsync();
                List<ComponentInfo> list = new List<ComponentInfo>();
                while(await reader.ReadAsync())
                {
                    list.Add(new ComponentInfo(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
                }
                return list;
            }
        }

        /// <summary>
        /// Gets a list of changesets performed to the component
        /// Sorted by the latest change first
        /// </summary>
        /// <param name="componentId">The id of the component to get the revision for</param>
        /// <returns>A list of changesets</returns>
        public async Task<RevisionInfo[]> GetRevisions(int componentId)
        {
            using(Connection conn = await connector.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "select `revisionNumber`, `userID`, `type`, `comment`, `timestamp` from componentsRevisions where `staticComponentId` = @componentId;";
                command.Parameters.AddWithValue("@componentId", componentId);
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
        /// Rolls back the component to a previous revision 
        /// </summary>
        /// <param name="revisionId">The revision Id to roll back to</param>
        /// <param name="userId">The id of the user performing the rollback</param>
        /// <param name="comment">The reason for the rollback</param>
        /// <returns>Always true</returns>
        public async Task<bool> RollbackComponent(int revisionId, string userId, string comment)
        {
            using (Connection conn = await connector.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "rollbackComponent";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@revisionID", revisionId);
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@commentparam", comment);
                await command.ExecuteNonQueryAsync();
                return true;
            }
        }

        /// <summary>
        /// Updates a component
        /// </summary>
        /// <param name="activeComponentID">The ID of the component to update</param>
        /// <param name="component">The new values to be taken by the component</param>
        /// <param name="userid">The id of the user updating the component</param>
        /// <param name="comment">The reason behind the update</param>
        /// <returns>Always true</returns>
        public async Task<bool> UpdateComponent(int activeComponentID, ComponentInfo component, string userid, string comment)
        {
            using (Connection conn = await connector.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "updateComponent";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@componentId", component.id);
                command.Parameters.AddWithValue("@status", component.status);
                command.Parameters.AddWithValue("@associatedComponentType", activeComponentID);
                command.Parameters.AddWithValue("@comment", component.comment);
                command.Parameters.AddWithValue("@userID", userid);
                command.Parameters.AddWithValue("@logComment", comment);
                await command.ExecuteNonQueryAsync();
                return true;
            }
        }
    }
}
