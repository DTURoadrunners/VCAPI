using VCAPI.MockRepository;
using Xunit;

namespace VCAPI.Repository.ControllerTests
{
    public class CategoryControllerTest
    {
        private readonly ProjectMockRepository repository;
        public CategoryControllerTest()
        {
            repository = new ProjectMockRepository();
        }

        [Fact]
        public void ReturnsProjectGivenRegisteredId()
        {
            
        }
    }
}