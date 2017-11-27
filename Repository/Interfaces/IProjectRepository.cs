using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    
    public interface IProjectRepository
    {
         Task<bool> CreateProject(LogInfo log, string name);
         Task<bool> DeleteProject(int id, LogInfo log);
         Task<ProjectInfo> GetProject(int id);
         Task<List<ProjectInfo>> GetProjects();
         Task<bool> UpdateProject(ProjectInfo inf, int id, LogInfo log);
         Task<bool> RollbackProject(int id, LogInfo log);
    }
}