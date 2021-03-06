﻿using System;
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
    public class MySQLUserRepository : IUserRepository
    {
        /// <summary>
        /// Hard coded settings for the hash
        /// Should be moved to config file
        /// </summary>
        private struct HashSettings{
            // Hard-coded hash.. for now.
            public const string salt = "oihergoi23r092ugwe";
            public const int hashSize = 512;
            public const int iterations = 10000;
        }

        
        readonly byte[] bsalt;
        readonly DatabaseConnector connector;
        public MySQLUserRepository(DatabaseConnector conn)
        {
            bsalt = Encoding.UTF32.GetBytes(HashSettings.salt);
            connector = conn;
        }  

        /// <summary>
        /// Checks if a user's password combination is correct
        /// </summary>
        /// <param name="username">The username to authenticate</param>
        /// <param name="password">The password to authenticate</param>
        /// <returns>True if authenticated, false otherwise</returns>
        public async Task<bool> Authenticate(string username, string password)
        {
           UserInfo info = await GetUser(username);
           if(info != null){
            byte[] hash = (KeyDerivation.Pbkdf2(password, bsalt, KeyDerivationPrf.HMACSHA512, HashSettings.iterations, HashSettings.hashSize));
            return (hash.SequenceEqual(info.password));
           }
           else{
               return false;
           }
        }

        /// <summary>
        /// Gets a user's information
        /// </summary>
        /// <param name="username">The user to retrieve information from</param>
        /// <returns>The user's information</returns>
        public async Task<UserInfo> GetUser(string username)
        {
            using(Connection conn = await connector.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "getUser";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@userID", username);
                command.Parameters["@userID"].Direction = ParameterDirection.Input;
                DbDataReader reader = await command.ExecuteReaderAsync();
                if(!reader.Read()){
                    return null;
                }
                UserInfo info = new UserInfo{ 
                    userID = reader.GetString(0), 
                    firstname = reader.GetString(1), 
                    lastname = reader.GetString(2),
                    phonenumber = reader.GetInt32(3),
                    password = Convert.FromBase64String(reader.GetString(4)),
                    superuser = reader.GetBoolean(5)
                };
                return info;
            }
        }

        /// <summary>
        /// Registers a user in the database and hashes the password
        /// </summary>
        /// <param name="info">The user to be created</param>
        /// <returns>True if successfull, false if user already exists</returns>
        public async Task<bool> CreateUser(UserInfo info)
        {
           using(Connection conn = await connector.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "createUser";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", info.userID);
                command.Parameters.AddWithValue("@firstnameparam", info.firstname);
                command.Parameters.AddWithValue("@lastnameparam", info.lastname);
                command.Parameters.AddWithValue("@phonenumberparam", 0);

                byte[] hash = (KeyDerivation.Pbkdf2(Encoding.UTF8.GetString(info.password), bsalt, KeyDerivationPrf.HMACSHA512, HashSettings.iterations, HashSettings.hashSize));

                string pass = Convert.ToBase64String(hash);
                command.Parameters.AddWithValue("@passwordparam", pass);
                try{
                    await command.ExecuteNonQueryAsync();
                    return true;
                }
                // Duplicate key.. most likely
                catch(MySqlException){
                    return false;
                }
           }
        }

        /// <summary>
        /// Updates the user's information -- Unfinished
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUser(UserInfo user)
        {
            using(Connection conn = await connector.Create()){
                MySqlCommand command = conn.Get().CreateCommand();
                command.CommandText = "updateUser";
                command.CommandType = CommandType.StoredProcedure;


                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}

