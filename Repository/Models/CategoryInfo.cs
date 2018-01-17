using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VCAPI.Repository.Models
{
    public class CategoryInfo
    {
        public int id;
        public string name;

        /// <summary>
        /// All the data needed to maintain a Category
        /// </summary>
        /// <param name="id">The unique id for the category</param>
        /// <param name="name">The name shown in the frontend</param>
        public CategoryInfo(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
