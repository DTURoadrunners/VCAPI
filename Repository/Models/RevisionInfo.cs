namespace VCAPI.Repository.Models
{
    /// <summary>
    /// Used in all Rollbacks and contains all the infomation needed.
    /// </summary>
    public class RevisionInfo
    {
        public int revisonId;
        public string author;
        public string eventType;
        public string comment;
        public int timestamp;
    }
}