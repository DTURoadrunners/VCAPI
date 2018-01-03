using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using tests.MockRepository;
using VCAPI.Controllers;
using VCAPI.MockRepository;
using VCAPI.Repository.Models;
using Xunit;

namespace VCAPI.Repository.ControllerTests
{
    public class CategoryControllerTest
    {
        private readonly ProjectMockRepository repository;
        private readonly ProjectController controller;
        private readonly MockResourceAccess access;
        private int existingProjectId;
        public CategoryControllerTest()
        {
            repository = new ProjectMockRepository();
            access = new MockResourceAccess();
            controller = new ProjectController(repository, access);
            InitializeRepository();
        }

        private async void InitializeRepository()
        {
            existingProjectId = await repository.CreateProject("Dummy project", "Nobody", "Created project");
        }

        void SetCallersUsername(string name)
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{
                new Claim(ClaimTypes.NameIdentifier, name)
                }));
        }

        [Fact]  
        public async void ReturnsProjectGivenValidId()
        {
            OkObjectResult result = await controller.getProject(existingProjectId) as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            ProjectInfo response = result.Value as ProjectInfo;
            Assert.NotNull(response);
        }
    }
}