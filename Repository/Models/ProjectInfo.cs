namespace VCAPI.Repository.Models
{
    public class ProjectInfo
    {
        public int id;
        public string name;

        public ProjectInfo(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}