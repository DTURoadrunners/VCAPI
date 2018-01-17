namespace VCAPI.Repository.Models
{
    public class ProjectInfo
    {
        public int id;
        public string name;

        /// <summary>
        /// The infomation a project needed to maintain a project
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="name">The name shown to the users</param>
        public ProjectInfo(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}