using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Creates a user
        /// </summary>
        /// <param name="info">The info needed to create it</param>
        /// <returns></returns>
        Task<bool> CreateUser(UserInfo info);
        Task<bool> Authenticate(string username, string password);
        Task<UserInfo> GetUser(string username);

        Task<bool> UpdateUser(UserInfo user);
    }
}
