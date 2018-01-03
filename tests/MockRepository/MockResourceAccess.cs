using System.Collections.Generic;
using System.Threading.Tasks;
using VCAPI.Repository;
using VCAPI.Repository.Interfaces;

namespace tests.MockRepository
{
    public class MockResourceAccess : IResourceAccess
    {

        private Dictionary<string, RANK> repository = new Dictionary<string, RANK>();
        private HashSet<string> superAdmins = new HashSet<string>();

        public async Task<bool> AssignRankForProject(string username, int projId, RANK rank)
        {
            repository[username + projId] = rank;
            return true;
        }

        public async Task<RANK> GetRankForProject(string username, int projId)
        {
            if(await IsSuperAdmin(username))
                return RANK.SUPERADMIN;

            if(!repository.ContainsKey(username + projId))
                return RANK.GUEST;
            
            return repository[username + projId];
        }

        public void AddSuperadmin(string name)
        {
            superAdmins.Add(name);
        }
        public async Task<bool> IsSuperAdmin(string username)
        {
            if(!superAdmins.Contains(username))
            {
                return false;
            }
            return true;
        }
    }
}