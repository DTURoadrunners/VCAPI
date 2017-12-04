using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface IComponentTypeRepository
    {
<<<<<<< HEAD
        Task<int> CreateComponentType(ComponentTypeInfo info, string userId, string comment, int id);
        Task<bool> UpdateComponentType(ComponentTypeInfo info, string userId, string comment, int ID);
        Task<bool> DeleteComponentType(int ID, string userId, string comment);
        Task<bool> RollbackComponentType(int ID, string userId, string comment);
        Task<ComponentTypeInfo> GetComponentType(int Id);
=======
        Task<int> CreateComponentType(ComponentTypeInfo info, int projectId, string userId, string comment);
        Task<bool> UpdateComponentType(ComponentTypeInfo info, int projectId, string userId, string comment);
        Task<bool> DeleteComponentType(int id, string userId, string comment);
        Task<bool> RollbackComponentType(int id, string userId, string comment);
        Task<ComponentTypeInfo> GetComponentType(int id);
>>>>>>> b92f61bfb42939096633cbc10ff5f0f062c63c66
        Task<List<ComponentTypeInfo>> GetComponentTypes(int projectId);
    }
}
