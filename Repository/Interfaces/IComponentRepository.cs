using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface IComponentRepository
    {
        Task<int> CreateComponent(int activeComponentTypeID, ComponentInfo component, string userid, string comment);
        Task<ComponentInfo> GetComponent(int id);
        Task<List<ComponentInfo>> GetComponents(int id);
        Task<bool> UpdateComponent(int activeComponentTypeID, ComponentInfo component, string userid, string comment);
        Task<bool> DeleteComponent(int activeID, string userid, string comment);
        Task<bool> RollbackComponent(int projectId, int revisionId, string userId, string comment);
        Task<RevisionInfo[]> GetRevisions(int componentTypeId);
    }
}
