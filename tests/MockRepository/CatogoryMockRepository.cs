using System.Collections.Generic;
using System.Threading.Tasks;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;

namespace tests.MockRepository
{
    public class CatogoryMockRepository : ICategoryRepository
    {
        private List<CategoryInfo> repository = new List<CategoryInfo>();
        
        public bool RepositoryContainsEntry(int id)
        {
            return id < repository.Count && repository[id] != null;
        }
        public int GetNextInsertId()
        {
            return repository.Count;
        }
        public async Task<int> CreateCategory(int projectId, CategoryInfo model, string userId, string comment)
        {
            int createdIndex = GetNextInsertId();
            repository.Add(model);
            return createdIndex;
        }

        public async Task<List<CategoryInfo>> GetCategories(int projectId)
        {
            return new List<CategoryInfo>(repository);
        }

        public async Task<CategoryInfo> GetCategory(int id)
        {
           if(RepositoryContainsEntry(id))
            {
                return repository[id];
            }
            return null;
        }
    }
}