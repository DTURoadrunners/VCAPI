using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    
    public interface IProjectRepository
    {
         Task<int> CreateProject(ProjectInfo inf);
         Task<bool> DeleteProject(int id);
         Task<ProjectInfo> GetProject(int id);
         Task<bool> UpdateProject(ProjectInfo inf, int id);
    }
}