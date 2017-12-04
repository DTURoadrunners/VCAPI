using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task<int> CreateCategory(int projectId, CategoryInfo model, string userId, string comment);
        Task<CategoryInfo> GetCategory(int id);
        Task<List<CategoryInfo>> GetCategories(int projectId);
    }
}
