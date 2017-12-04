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
    public class MySQLDocumentRepository : IDocumentRepository
    {
        readonly DatabaseConnector connection;
        public MySQLDocumentRepository(DatabaseConnector conn)
        {
            connection = conn;
        }

        public async Task<int> CreateDocument(DocumentInfo info, string userId, string comment, int id)
        {
            using(Connection conn = await connection.Create())
            {
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "creatDocument";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@_filename", info.filename);
                command.Parameters.AddWithValue("@_activeComponentTypeID", id);
                command.Parameters.AddWithValue("@_bucketpath", info.bucketpath);
                command.Parameters.AddWithValue("@_description", info.description);
                command.Parameters.AddWithValue("@userID", userId);
                command.Parameters.AddWithValue("@logComment", comment);

                DbDataReader reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    return reader.GetInt32(0);
                else
                    return -1;
            }
        }

         public async Task<DocumentInfo> GetDocument(int id)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "getDocument";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", id);
                command.Parameters["@ID"].Direction = ParameterDirection.Input;
                DbDataReader reader = await command.ExecuteReaderAsync();
                if(!await reader.NextResultAsync()){
                    return null;
                }
                
                return new DocumentInfo(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
            }
        }

        public async Task<List<DocumentInfo>> getDocuments(int id)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "getActiveDocuments";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@componentTypeID", id);
                command.Parameters["@ID"].Direction = ParameterDirection.Input;
                DbDataReader reader = await command.ExecuteReaderAsync();
                if(!await reader.NextResultAsync()){
                    return null;
                }
                List <DocumentInfo> list = new List<DocumentInfo>();
                while (reader.NextResult()){
                    list.Add(new DocumentInfo(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
                }
                
                return list;
            }
        }
        
        public async Task<bool> UpdateDocument(DocumentInfo info, string userId, string comment)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "updateDocument";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@activeID", info.id);
                command.Parameters.AddWithValue("@filename", info.filename);
                command.Parameters.AddWithValue("@bucketPath", info.bucketpath);
                command.Parameters.AddWithValue("@description", info.description);
                command.Parameters.AddWithValue("@userID", userId);
                command.Parameters.AddWithValue("@logComment", comment);
               
               return await command.ExecuteNonQueryAsync() == 1;
            }
        }
        public async Task<bool> DeleteDocument(int id, string userId, string comment)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "deleteDocument";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@activeID", id);
                command.Parameters.AddWithValue("@userID", userId);
                command.Parameters.AddWithValue("@logComment", comment);
               
               return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> RollbackDocument(int id, string userId, string comment)
        {
            using(Connection conn = await connection.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "rollbackDocument";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@logID", id);
                command.Parameters.AddWithValue("@userID", userId);
                command.Parameters.AddWithValue("@commentParam", comment);
               
               return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }       
}
