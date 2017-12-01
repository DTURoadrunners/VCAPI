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