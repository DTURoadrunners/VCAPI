using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    
    public interface IProjectRepository
    {
        /// <summary>
        /// Creates a project
        /// </summary>
        /// <param name="name">Name of the project(they start of empty)</param>
        /// <param name="userId">Checks if the user is a superadmin(the only rank that can create projects)</param>
        /// <param name="comment">Used in logs</param>
        /// <returns></returns>
         Task<int> CreateProject(string name, string userId, string comment);
         Task<bool> DeleteProject(int id, string userId, string comment);
         Task<ProjectInfo> GetProject(int id);
         Task<List<ProjectInfo>> GetProjects();
         Task<bool> UpdateProject(ProjectInfo inf, int id, string userId, string comment);
         Task<RevisionInfo[]> GetRevisions(int projectId);
         Task<bool> RollbackProject(int projectId, int revisionId, string userId, string comment);
    }
}