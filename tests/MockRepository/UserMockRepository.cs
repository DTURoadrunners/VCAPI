using System.Collections.Generic;
using System.Threading.Tasks;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;

namespace tests.MockRepository
{
    public class UserMockRepository : IUserRepository
    {
        private List<UserInfo> repository = new List<UserInfo>();
        
        public bool RepositoryContainsEntry(int id)
        {
            return id < repository.Count && repository[id] != null;
        }
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
            repository.Add(info);
            return createdIndex;
        }

        public async Task<UserInfo> GetUser(string username)
        {  
           if(RepositoryContainsEntry(username))
            {
                return repository[username];
            }
            return null;
        }

        public async Task<bool> UpdateUser(UserInfo user)
        {
           if(RepositoryContainsEntry(userID))
            {
                repository[userID] = user;
                return true;
            }
            return false;
        }
    }
}