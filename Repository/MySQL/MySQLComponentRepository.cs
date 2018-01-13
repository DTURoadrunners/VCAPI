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
    public class MySQLComponentRepository : IComponentRepository
    {
        readonly DatabaseConnector connector;
        public MySQLComponentRepository(DatabaseConnector conn)
        {
            connector = conn;
        }

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

        public async Task<bool> RollbackComponent(int logId, string userId, string comment)
        {
            using (Connection conn = await connector.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "rollbackComponent";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@logID", logId);
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@ommentparam", comment);
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateComponent(int activeComponentTypeID, ComponentInfo component, string userid, string comment)
        {
            using (Connection conn = await connector.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "updateComponent";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@componentId", component.id);
                command.Parameters.AddWithValue("@status", component.status);
                command.Parameters.AddWithValue("@associatedComponentType", activeComponentTypeID);
                command.Parameters.AddWithValue("@comment", component.comment);
                command.Parameters.AddWithValue("@userID", userid);
                command.Parameters.AddWithValue("@logComment", comment);
                await command.ExecuteNonQueryAsync();
                return true;
            }
        }
    }
}
