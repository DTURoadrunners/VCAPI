using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface IComponentTypeRepository
    {
        Task<int> CreateComponentType(ComponentTypeInfo info, string userId, string comment, int id);
        Task<bool> UpdateComponentType(ComponentTypeInfo info, string userId, string comment, int ID);
        Task<bool> DeleteComponentType(int ID, string userId, string comment);
        Task<bool> RollbackComponentType(int ID, string userId, string comment);
        Task<ComponentTypeInfo> GetComponentType(int Id);
        Task<List<ComponentTypeInfo>> GetComponentTypes(int projectId);
    }
}
