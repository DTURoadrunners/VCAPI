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

        public async Task<int> CreateComponent(int activeComponentTypeID, ComponentInfo component, string userid, string comment)
        {
            using (Connection conn = await connector.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "createComponent";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@activeComponentTypeID", activeComponentTypeID);
                command.Parameters.AddWithValue("@status", component.status);
                command.Parameters.AddWithValue("@comment", component.comment);
                command.Parameters.AddWithValue("@userID", userid);
                command.Parameters.AddWithValue("@logComment", comment);
                return await command.ExecuteNonQueryAsync(); //UNDONE: This always returns -1, how do we return the id of the component?
                //return (int) await command.ExecuteScalarAsync() //Maybe? This gets the first column of the inserted row
            }
        }

        public async Task<bool> DeleteComponent(int activeID, string userid, string comment)
        {
            using (Connection conn = await connector.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "createComponent";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@activeID", activeID);
                command.Parameters.AddWithValue("@userID", userid);
                command.Parameters.AddWithValue("@logComment", comment);
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<ComponentInfo> GetComponent(int id)
        {
            using (Connection conn = await connector.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "getComponent";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", id);
                
                DbDataReader reader = await command.ExecuteReaderAsync();
                if (!await reader.NextResultAsync())
                {
                    return null;
                }
                return new ComponentInfo(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
            }
        }

        public async Task<List<ComponentInfo>> GetComponents(int id)
        {
            using (Connection conn = await connector.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "getActiveComponents";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@componentTypeID", id);
                command.Parameters["@ID"].Direction = ParameterDirection.Input;
                DbDataReader reader = await command.ExecuteReaderAsync();
                if (!await reader.NextResultAsync())
                {
                    return null;
                }
                List<ComponentInfo> list = new List<ComponentInfo>();
                while (reader.NextResult())
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
                command.Parameters.AddWithValue("@activeComponentTypeID", activeComponentTypeID);
                command.Parameters.AddWithValue("@status", component.status);
                command.Parameters.AddWithValue("@comment", component.comment);
                command.Parameters.AddWithValue("@userID", userid);
                command.Parameters.AddWithValue("@logComment", comment);
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
