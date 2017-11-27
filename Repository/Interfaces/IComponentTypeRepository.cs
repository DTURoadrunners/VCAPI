using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface IComponentTypeRepository
    {
        Task<bool> CreateComponentType(ComponentTypeInfo info, string userid, string comment, int id);
        Task<bool> UpdateComponentType(ComponentTypeInfo info, string userid, string comment, int activeID);
        Task<bool> DeleteComponentType(int ID, string userid, string comment);
        Task<bool> RollbackComponentType(int ID, string userid, string comment);
        Task<ComponentTypeInfo> GetComponentType(int Id);
        Task<List<ComponentTypeInfo>> GetComponentTypes(int projectId);
    }
}
