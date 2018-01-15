using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface IComponentTypeRepository
    {
        Task<int> CreateComponentType(ComponentTypeInfo info, int projectId, string userId, string comment);
        Task<bool> UpdateComponentType(ComponentTypeInfo info, string userId, string comment);
        Task<bool> DeleteComponentType(int id, string userId, string comment);
        Task<RevisionInfo[]> GetRevisionAsync(int componentTypeId);
        Task<bool> RollbackComponentType(int projectId, int revisionId, string userId, string comment);
        Task<ComponentTypeInfo> GetComponentType(int id, int projectId);
        Task<List<ComponentTypeInfo>> GetComponentTypes(int projectId);
    }
}
