using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;

namespace VCAPI.MockRepository
{
    public class ProjectMockRepository : IProjectRepository
    {
        private List<ProjectInfo> repository = new List<ProjectInfo>();

        public Boolean rollback{
            private set;
            get;
        }
        public bool RepositoryContainsEntry(int id)
        {
            return id < repository.Count && repository[id] != null;
        }

        public int GetNextInsertId()
        {
            return repository.Count;
        }

        public async Task<int> CreateProject(string name, string userId, string comment)
        {
            int createdIndex = GetNextInsertId();
            ProjectInfo newProject = new ProjectInfo(createdIndex, name);
            repository.Add(newProject);
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

        public async Task<bool> RollbackProject(int id, int revisionId, string userId, string comment)
        {
            rollback = true;
            return rollback;
        }

        public async Task<bool> UpdateProject(ProjectInfo inf, int id, string userId, string comment)
        {
            if(RepositoryContainsEntry(id))
            {
                repository[id] = inf;
                return true;
            }
            return false;
        }
    }
}