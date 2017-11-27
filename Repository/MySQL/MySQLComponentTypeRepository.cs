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
    public class MySQLComponentTypeRepository : IComponentTypeRepository
    {
        DatabaseConnector connection;
        public MySQLComponentTypeRepository(DatabaseConnector conn)
        {
            connection = conn;
        }

        public async Task<bool> CreateComponentType(ComponentTypeInfo info, LogInfo log)
        {
            using(Connection conn = await connection.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "creatComponenttype";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@nameparam", info.name);
                command.Parameters.AddWithValue("@activeProjectIDParam", log.activeProjectIDParam);
                command.Parameters.AddWithValue("@categoryID", info.categoryID);
                command.Parameters.AddWithValue("@storageparam", info.storage);
                command.Parameters.AddWithValue("@description", info.description);
                command.Parameters.AddWithValue("@userid", log.userID);
                command.Parameters.AddWithValue("@commentparam", log.comment);

                return await command.ExecuteNonQueryAsync() == 1;          
            }
        }

         public async Task<ComponentTypeInfo> GetComponentType(int Id)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "getComponenttype";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", Id);
                command.Parameters["@ID"].Direction = ParameterDirection.Input;
                DbDataReader reader = await command.ExecuteReaderAsync();
                if(!await reader.NextResultAsync()){
                    return null;
                }

                ComponentTypeInfo info = new ComponentTypeInfo();
                info.id = reader.GetInt32(0);
                info.name = reader.GetString(1);
                info.categoryID = reader.GetInt32(2);
                info.storage = reader.GetInt32(3);
                info.description = reader.GetString(4);
                return info;
            }
        }

        public async Task<List<ComponentTypeInfo>> GetComponentTypes(int projectId)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "getActiveComponenttypes";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@projectID", projectId);
                command.Parameters["@ID"].Direction = ParameterDirection.Input;
                DbDataReader reader = await command.ExecuteReaderAsync();
                if(!await reader.NextResultAsync()){
                    return null;
                }
                List <ComponentTypeInfo> list = new List<ComponentTypeInfo>();
                while (reader.NextResult()){
                    ComponentTypeInfo info = new ComponentTypeInfo();
                    info.id = reader.GetInt32(0);
                    info.name = reader.GetString(1);
                    info.categoryID = reader.GetInt32(2);
                    info.storage = reader.GetInt32(3);
                    info.description = reader.GetString(4);
                    list.Add(info);
                }
                
                return list;
            }
        }
        
        public async Task<bool> UpdateComponentType(ComponentTypeInfo info, LogInfo log)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "updateComponenttype";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@nameparam", info.name);
                command.Parameters.AddWithValue("@activeID", log.activeID);
                command.Parameters.AddWithValue("@categoryID", info.categoryID);
                command.Parameters.AddWithValue("@storageparam", info.storage);
                command.Parameters.AddWithValue("@descriptionparam", info.description);
                command.Parameters.AddWithValue("@userid", log.userID);
                command.Parameters.AddWithValue("@commentparam", log.comment);
               
               return await command.ExecuteNonQueryAsync() == 1;
            }
        }
        public async Task<bool> DeleteComponentType(int ID, LogInfo log)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "deleteComponenttype";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@activeComponentTypeID", ID);
                command.Parameters.AddWithValue("@userid", log.userID);
                command.Parameters.AddWithValue("@commentparam", log.comment);
               
               return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> RollbackComponentType(int ID, LogInfo log)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "rollbackComponenttype";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@logID", ID);
                command.Parameters.AddWithValue("@userid", log.userID);
                command.Parameters.AddWithValue("@commentparam", log.comment);
               
               return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }       
}