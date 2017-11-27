using System.Threading.Tasks;

namespace VCAPI.Repository.Interfaces
{
    public interface IResourceAccess
    {
        Task<RANK> GetRankForProject(string username, int projId);
        Task<bool> IsSuperAdmin(string username);
    }
}