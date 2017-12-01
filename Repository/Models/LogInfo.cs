using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VCAPI.Repository.Models
{
    public class LogInfo
    {
        public int logId;
        public string userID;
        public string comment;
        public int activeID;
        public int timestamp;
        public string type;
        public int id;

        public LogInfo(int logId, string userID, string comment, int activeID, int timestamp, string type, int id)
        {
            this.logId = logId;
            this.userID = userID;
            this.comment = comment;
            this.activeID = activeID;
            this.timestamp = timestamp;
            this.type = type;
            this.id = id;
        }
    };
}
