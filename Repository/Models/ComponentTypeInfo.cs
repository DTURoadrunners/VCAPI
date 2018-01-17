using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VCAPI.Repository.Models
{
    public class ComponentTypeInfo
    {
        public int id;
        public string name;
        public int categoryID;
        public int storage;
        public string description;
        
        /// <summary>
        /// All the infomation needed to maintain a ComponentType
        /// </summary>
        /// <param name="id">The unique id given</param>
        /// <param name="name">The name of the type</param>
        /// <param name="categoryID">The id of the type</param>
        /// <param name="storage">Shows how many is stored</param>
        /// <param name="description">Description of the part in detail</param>
        public ComponentTypeInfo(int id, string name, int categoryID, int storage, string description)
        {
            this.id = id;
            this.name = name;
            this.categoryID = categoryID;
            this.storage = storage;
            this.description = description;
        }
    }
}