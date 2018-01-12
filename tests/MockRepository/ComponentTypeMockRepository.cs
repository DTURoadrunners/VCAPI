using System.Collections.Generic;
using System.Threading.Tasks;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;

namespace tests.MockRepository
{
    public class ComponentTypeMockRepository : IComponentTypeRepository
    {
        private List<ComponentTypeInfo> repository = new List<ComponentTypeInfo>();
        public bool RepositoryContainsEntry(int id)
        {
            return id < repository.Count && repository[id] != null;
        }

        public int GetNextInsertId()
        {
            return repository.Count;
        }

        public async Task<int> CreateComponentType(ComponentTypeInfo info, int projectId, string userId, string comment)
        {
            int createdIndex = GetNextInsertId();
            repository.Add(info);
            return createdIndex;
        }

        public async Task<bool> UpdateComponentType(ComponentTypeInfo info, string userId, string comment)
        {
           if(RepositoryContainsEntry(info.id))
            {
                repository[info.id] = info;
                return true;
            }
            return false;
        }

        public async Task<ComponentTypeInfo> GetComponentType(int id, int projectId)
        {
            if(RepositoryContainsEntry(projectId))
            {
                return repository[id];
            }
            return null;
        }

        public async Task<List<ComponentTypeInfo>> GetComponentTypes(int projectId)
        {
             return new List<ComponentTypeInfo>(repository);
        }

        public async Task<bool> DeleteComponentType(int id, string userId, string comment)
        {
           if(RepositoryContainsEntry(id))
            {
                repository[id] = null;
                return true;
            }
            return false;
        }
        public async Task<bool> RollbackComponentType(int id, string userId, string comment)
        {
            throw new System.NotImplementedException();
        }

    }
}