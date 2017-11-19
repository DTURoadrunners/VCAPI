using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<UserInfo> CreateUser(string username, string password);
    }
}
