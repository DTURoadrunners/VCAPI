using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface IComponentTypeRepository
    {
        Task<bool> CreateComponentType(ComponentTypeInfo info, LogInfo log, int id);
        Task<bool> UpdateComponentType(ComponentTypeInfo info, LogInfo log);
        Task<bool> DeleteComponentType(int ID, LogInfo log);
        Task<bool> RollbackComponentType(int ID, LogInfo log);
        Task<ComponentTypeInfo> GetComponentType(int Id);
        Task<List<ComponentTypeInfo>> GetComponentTypes(int projectId);
    }
}
