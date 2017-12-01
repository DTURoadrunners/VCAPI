using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VCAPI.Repository.Models
{
    public class ComponentInfo
    {
        public int id;
        public string status;
        public string comment;

        public ComponentInfo(int id, string status, string comment)
        {
            this.id = id;
            this.status = status;
            this.comment = comment;
        }
    }
}
