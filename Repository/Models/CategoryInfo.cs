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

        public CategoryInfo(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
