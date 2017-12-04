using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    
    public interface IProjectRepository
    {
         Task<int> CreateProject(string name, string userId, string comment);
         Task<bool> DeleteProject(int id, string userId, string comment);
         Task<ProjectInfo> GetProject(int id);
         Task<List<ProjectInfo>> GetProjects();
         Task<bool> UpdateProject(ProjectInfo inf, int id, string userId, string comment);
         Task<bool> RollbackProject(int id, string userId, string comment);
    }
}