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
    public class MySQLComponent : IComponent
    {
        readonly DatabaseConnector connector;

        public async Task<bool> CreateComponent(int activeComponentTypeID, Component component, LogInfo log)
        {
            using (Connection conn = await connector.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "createComponent";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@activeComponentTypeID", activeComponentTypeID);
                command.Parameters.AddWithValue("@status", component.status);
                command.Parameters.AddWithValue("@comment", component.comment);
                command.Parameters.AddWithValue("@userID", log.userID);
                command.Parameters.AddWithValue("@logComment", log.comment);
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> DeleteComponent(string activeID, LogInfo log)
        {
            using (Connection conn = await connector.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "createComponent";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@activeID", activeID);
                command.Parameters.AddWithValue("@userID", log.userID);
                command.Parameters.AddWithValue("@logComment", log.comment);
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<Component> ReadComponent(string id)
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
                Component result = new Component();
                result.id = reader.GetString(0);
                result.status = reader.GetString(1);
                result.comment = reader.GetString(2);
                return result;
            }
        }

        public async Task<bool> UpdateComponent(int activeComponentTypeID, Component component, LogInfo log)
        {
            using (Connection conn = await connector.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "updateComponent";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@activeComponentTypeID", activeComponentTypeID);
                command.Parameters.AddWithValue("@status", component.status);
                command.Parameters.AddWithValue("@comment", component.comment);
                command.Parameters.AddWithValue("@userID", log.userID);
                command.Parameters.AddWithValue("@logComment", log.comment);
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
