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

         public async Task<ComponentTypeInfo> GetComponentType(int componentTypeId, int projectId)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "select ID, name, categoryID, storage, description from componentTypes where ID = @ID AND associatedProject = @projectId";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", componentTypeId);
                command.Parameters.AddWithValue("@projectId", projectId);
                command.Prepare();
                DbDataReader reader = await command.ExecuteReaderAsync();
                if(!await reader.ReadAsync()){
                    return null;
                }
                
                return new ComponentTypeInfo(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetString(4));
            }
        }

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
        
        public async Task<bool> UpdateComponentType(ComponentTypeInfo info, string userId, string comment)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "updateComponenttype";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@nameparam", info.name);
                command.Parameters.AddWithValue("@componentTypeId", info.id);
                command.Parameters.AddWithValue("@categoryID", info.categoryID);
                command.Parameters.AddWithValue("@storageparam", info.storage);
                command.Parameters.AddWithValue("@descriptionparam", info.description);
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@commentparam", comment);
                await command.ExecuteNonQueryAsync();
                return true;
            }
        }
        public async Task<bool> DeleteComponentType(int projectId, string userId, string comment)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "deleteComponenttype";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@activeComponentTypeID", projectId);
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@commentparam", comment);
                await command.ExecuteNonQueryAsync();
                return true;
            }
        }

        public async Task<bool> RollbackComponentType(int ID, string userId, string comment)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "rollbackComponenttype";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@logID", ID);
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@commentparam", comment);
               
               return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }       
}
