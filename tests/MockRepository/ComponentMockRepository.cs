using System.Collections.Generic;
using System.Threading.Tasks;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;

namespace tests.MockRepository
{
    public class ComponentMockRepository : IComponentRepository
    {
        private List<ComponentInfo> repository = new List<ComponentInfo>();
        public bool RepositoryContainsEntry(int id)
        {
            return id < repository.Count && repository[id] != null;
        }

        public int GetNextInsertId()
        {
            return repository.Count;
        }

        public async Task<int> CreateComponent(int activeComponentTypeID, ComponentInfo component, string userid, string comment)
        {
            int createdIndex = GetNextInsertId();
            repository.Add(component);
            return createdIndex;
        }

        public async Task<ComponentInfo> GetComponent(int id)
        {
            if(RepositoryContainsEntry(id))
            {
                return repository[id];
            }
            return null;
        }

        public async Task<List<ComponentInfo>> GetComponents(int id)
        {
           return new List<ComponentInfo>(repository);
        }

        public async Task<bool> UpdateComponent(int activeComponentTypeID, ComponentInfo component, string userid, string comment)
        {
            if(RepositoryContainsEntry(activeComponentTypeID))
            {
                repository[activeComponentTypeID] = component;
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteComponent(int activeID, string userid, string comment)
        {
             if(RepositoryContainsEntry(activeID))
            {
                repository[activeID] = null;
                return true;
            }
            return false;
        }

        public async Task<bool> RollbackComponent(int id, string userId, string comment)
        {
            throw new System.NotImplementedException();
        }
    }
}