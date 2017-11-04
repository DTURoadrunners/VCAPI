using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.MySQL
{
    public class MySQLUserRepository : IUserRepository
    {
        public UserInfo CreateUser(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
