using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VCAPI.Repository.Models
{
    public class DocumentInfo
    {
        public int id;
        public string filename;
        public string description;
        public string bucketpath;

        /// <summary>
        /// an empty constructor
        /// </summary>
        public DocumentInfo()
        {
            
        }
        /// <summary>
        /// The most used constructor
        /// </summary>
        /// <param name="id">The unique id given</param>
        /// <param name="filename">The name of the file</param>
        /// <param name="description">Description about the use of the document</param>
        /// <param name="bucketpath">Where the file is placed in the filing system</param>
        public DocumentInfo(int id, string filename, string description, string bucketpath)
        {
            this.id = id;
            this.filename = filename;
            this.description = description;
            this.bucketpath = bucketpath;
        }
    }
}