using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace VCAPI.Repository.MySQL
{
    public class DatabaseConnector
    {
        public class Connection : IDisposable
        {
            private readonly MySqlConnection connection;

            public MySqlConnection Get() => connection;

            public Connection(string cs)
            {
                connection = new MySqlConnection(cs);
            }

            void IDisposable.Dispose()
            {
                connection.Close();
            }
        }

        private readonly string cs;

        public DatabaseConnector(string connectionString)
        {
            cs = connectionString;
        }

        public async Task<Connection> Create()
        {
            Connection con = new Connection(cs);
            await con.Get().OpenAsync();
            return con;
        }
    }
}
