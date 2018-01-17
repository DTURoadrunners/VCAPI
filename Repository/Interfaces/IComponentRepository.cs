using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface IComponentRepository
    {
        /// <summary>
        /// A component is all the components used in the project, it has an id and its divided into types 
        /// </summary>
        /// <param name="activeComponentTypeID">Used to see if its in </param>
        /// <param name="component">The data for the component</param>
        /// <param name="userid">Checks to see if the user is eligible</param>
        /// <param name="comment">Used in logs</param>
        /// <returns></returns>
        Task<int> CreateComponent(int activeComponentTypeID, ComponentInfo component, string userid, string comment);
        Task<ComponentInfo> GetComponent(int id);
        Task<List<ComponentInfo>> GetComponents(int id);
        Task<bool> UpdateComponent(int activeComponentTypeID, ComponentInfo component, string userid, string comment);
        Task<bool> DeleteComponent(int activeID, string userid, string comment);
        Task<bool> RollbackComponent(int projectId, int revisionId, string userId, string comment);
        Task<RevisionInfo[]> GetRevisions(int componentTypeId);
    }
}
