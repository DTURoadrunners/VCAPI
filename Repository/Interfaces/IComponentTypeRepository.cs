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
        Task<bool> UpdateComponentType(ComponentTypeInfo info, int projectId, string userId, string comment);
        Task<bool> DeleteComponentType(int id, string userId, string comment);
        Task<bool> RollbackComponentType(int id, string userId, string comment);
        Task<ComponentTypeInfo> GetComponentType(int id);
        Task<List<ComponentTypeInfo>> GetComponentTypes(int projectId);
    }
}
