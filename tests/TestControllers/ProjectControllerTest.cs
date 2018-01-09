using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tests.MockRepository;
using tests.TestControllers;
using VCAPI.Controllers;
using VCAPI.MockRepository;
using VCAPI.Repository.Models;
using Xunit;

namespace VCAPI.Repository.ControllerTests
{
    public class ProjectControllerTest
    {
        private readonly ProjectMockRepository repository;
        private readonly ProjectController controller;
        private readonly MockResourceAccess access;
        private int existingProjectId;
        public ProjectControllerTest()
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

        int GetCreateLocationId(string location)
        {
            int seperatorIndex = location.LastIndexOf('/');
            string idString = location.Substring(seperatorIndex + 1);
            int id;
            if(int.TryParse(idString, out id))
            {
                return id;
            }
            return -1;
        }
        [Fact]  

        public async void ReturnsProjectGivenValidId()
        {
            OkObjectResult result = await controller.getProject(existingProjectId) as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

            ProjectInfo response = result.Value as ProjectInfo;
            Assert.NotNull(response);
        }

        [Fact]
        public async void ReturnsAllProjects()
        {
            OkObjectResult result = await controller.getAllProjects() as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            List<ProjectInfo> response = result.Value as List<ProjectInfo>;
            Assert.NotNull(response);
            Assert.Equal(1, response.Count);
        }

        [Fact]
        public async void CreatesProjectGivenCorrectModel()
        {
            string username = "Nobody";
            access.AddSuperadmin(username);
            ControllerTestUtility.SetCallersUsername(username, controller);

            ProjectInfo info = new ProjectInfo(0, "TestProject");
            int expectedCreateId = repository.GetNextInsertId();
            ProjectController.CreateProjectMarshall marshall;
            marshall.info = info;
            marshall.comment = "Initialize create";
            CreatedResult result = await controller.createProject(marshall) as CreatedResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
            int? createdId = result.Value as int?;
            Assert.NotNull(createdId);
            Assert.Equal(expectedCreateId, createdId);
            Assert.True(repository.RepositoryContainsEntry((int)createdId));
        }

        [Fact]
        public async void UpdatesProjectIfSuperadmin()
        {
            const string username = "Nobody";
            const string newProjectName = "UpdatedProject";
            access.AddSuperadmin(username);
            ControllerTestUtility.SetCallersUsername(username, controller);
            ProjectInfo info = new ProjectInfo(0, newProjectName);
            OkResult result = await controller.updateProject(existingProjectId, info, "Typo") as OkResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

            ProjectInfo repositoryEntry = await repository.GetProject(existingProjectId);
            Assert.NotNull(repositoryEntry);
            Assert.Equal(newProjectName, repositoryEntry.name);
        }

        [Fact]
        public async void DeletesProjectIfSuperadmin()
        {
            string username = "Nobody";
            access.AddSuperadmin(username);
            ControllerTestUtility.SetCallersUsername(username, controller);

            OkResult result = await controller.deleteProject(existingProjectId, "Unnecessary project") as OkResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

            Assert.Null(await repository.GetProject(existingProjectId));    
        }
    }
}