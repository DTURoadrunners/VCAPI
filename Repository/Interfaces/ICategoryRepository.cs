using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        /// <summary>
        /// Creates a component
        /// </summary>
        /// <param name="projectId">Used to see which project it is created in</param>
        /// <param name="model">Contains the data</param>
        /// <param name="userId">Checks if a user has the rank nessecary to create</param>
        /// <param name="comment">Used in the log</param>
        /// <returns></returns>
        Task<int> CreateCategory(int projectId, CategoryInfo model, string userId, string comment);
        Task<CategoryInfo> GetCategory(int id);
        Task<List<CategoryInfo>> GetCategories(int projectId);
    }
}
