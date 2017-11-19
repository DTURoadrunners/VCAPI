using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public enum RANK
    {
        GUEST,
    }
    public interface IUserRepository
    {
        Task<UserInfo> CreateUser(string username, string password);
        Task<RANK> GetRankForProject(string username, int projId);
        Task<bool> Authenticate(string username, string password);
    }
}
