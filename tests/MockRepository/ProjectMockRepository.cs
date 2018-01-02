using System.Collections.Generic;
using System.Threading.Tasks;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;

namespace VCAPI.MockRepository
{
    public class ProjectMockRepository : IProjectRepository
    {
        List<ProjectInfo> repository = new List<ProjectInfo>();
        private bool RepositoryContainsEntry(int id)
        {
            return id < repository.Count && repository[id] != null;
        }
        public async Task<int> CreateProject(string name, string userId, string comment)
        {
            int createdIndex = repository.Count;
            ProjectInfo newProject = new ProjectInfo(createdIndex, name);
            return createdIndex;
        }

        public async Task<bool> DeleteProject(int id, string userId, string comment)
        {
            if(RepositoryContainsEntry(id))
            {
                repository[id] = null;
                return true;
            }
            return false;
        }

        public async Task<ProjectInfo> GetProject(int id)
        {
            if(RepositoryContainsEntry(id))
            {
                return repository[id];
            }
            return null;
        }

        public async Task<List<ProjectInfo>> GetProjects()
        {
            return new List<ProjectInfo>(repository);
        }

        public Task<bool> RollbackProject(int id, string userId, string comment)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UpdateProject(ProjectInfo inf, int id, string userId, string comment)
        {
            throw new System.NotImplementedException();
        }
    }
}