using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    
    public interface IProjectRepository
    {
         Task<bool> CreateProject(string userid, string comment, string name);
         Task<bool> DeleteProject(int id, string userid, string comment);
         Task<ProjectInfo> GetProject(int id);
         Task<List<ProjectInfo>> GetProjects();
         Task<bool> UpdateProject(ProjectInfo inf, int id, string userid, string comment);
         Task<bool> RollbackProject(int id, string userid, string comment);
    }
}