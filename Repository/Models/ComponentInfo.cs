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

        /// <summary>
        /// All the info needed to maintain a Component
        /// </summary>
        /// <param name="id">The unique id given</param>
        /// <param name="status">An indication if the component is available or not</param>
        /// <param name="comment">Used for logs</param>
        public ComponentInfo(int id, string status, string comment)
        {
            this.id = id;
            this.status = status;
            this.comment = comment;
        }
    }
}
