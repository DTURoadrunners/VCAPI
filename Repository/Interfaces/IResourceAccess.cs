using System.Threading.Tasks;

namespace VCAPI.Repository.Interfaces
{
    /// <summary>
    /// Used in checking if a user has the rank in a project to do what he attempts to do.
    /// </summary>
    public interface IResourceAccess
    {
        Task<RANK> GetRankForProject(string username, int projId);
        Task<bool> AssignRankForProject(string username, int projId, RANK rank);
        Task<bool> IsSuperAdmin(string username);
    }
}