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
        public DocumentInfo()
        {
            
        }
        public DocumentInfo(int id, string filename, string description, string bucketpath)
        {
            this.id = id;
            this.filename = filename;
            this.description = description;
            this.bucketpath = bucketpath;
        }
    }
}