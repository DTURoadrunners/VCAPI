using System.Collections.Generic;
using System.Threading.Tasks;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;

namespace tests.MockRepository
{
    public class UserMockRepository : IUserRepository
    {
        private Dictionary<string, UserInfo> repository = new Dictionary<string, UserInfo>();
        
        public int GetNextInsertId()
        {
            return repository.Count;
        }
        public async Task<bool> Authenticate(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> CreateUser(UserInfo info)
        {
            int createdIndex = GetNextInsertId();
            repository[info.userID] = info;
            return true;
        }

        public async Task<UserInfo> GetUser(string username)
        {  
            UserInfo info;
            repository.TryGetValue(username, out info);
            return info;
        }

        public async Task<bool> UpdateUser(UserInfo user)
        {
            if(await GetUser(user.userID) != null)
            {
                repository[user.userID] = user;
                return true;
            }
            return false;
        }
    }
}