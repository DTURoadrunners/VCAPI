using System.Threading.Tasks;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.MySQL
{
    public class MySQLProjectRepository : IProjectRepository
    {
        readonly DatabaseConnector connector;
        public MySQLProjectRepository(DatabaseConnector conn)
        {
            connector = conn;
        }
        public Task<int> CreateProject(ProjectInfo inf)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteProject(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<ProjectInfo> GetProject(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UpdateProject(ProjectInfo inf, int id)
        {
            throw new System.NotImplementedException();
        }
    }
}