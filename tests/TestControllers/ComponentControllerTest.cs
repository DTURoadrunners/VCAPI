using Microsoft.AspNetCore.Mvc;
using tests.TestControllers;
using Xunit;
using VCAPI.Controllers;
using VCAPI.MockRepository;
using tests.MockRepository;
using System.Net;
using VCAPI.Repository.Models;
using System.Collections.Generic;

namespace VCAPI.Repository.ControllerTests
{
    public class ComponentControllerTest
    {
        private readonly ComponentMockRepository repository;
        private readonly ComponentController controller;
        private readonly MockResourceAccess access;
        private int existingComponentId;
        public ComponentControllerTest()
        {
            repository = new ComponentMockRepository();
            access = new MockResourceAccess();
            controller = new ComponentController(repository, access);
            InitializeRepository();
        }
        private async void InitializeRepository()
        {
            existingComponentId = await repository.CreateComponent(
                0, new ComponentInfo(10, "available", "comment"), "1", "comment");
        }

        [Fact]
        public async void GetComponentReturnsValidsID(){
            OkObjectResult result = await controller.GetComponent(existingComponentId) as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

            ComponentInfo response = result.Value as ComponentInfo;
            Assert.NotNull(response);
        }
        [Fact]
        public async void GetComponentReturnsBadRequest(){
            OkObjectResult result = await controller.GetComponent(100) as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);

            ComponentInfo response = result.Value as ComponentInfo;
            Assert.Null(response);
        }
        [Fact]
        public async void ReturnsAllComponents(){
            IActionResult result = await controller.GetComponents(0);
            OkObjectResult okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
            List<ComponentInfo> response = okResult.Value as List<ComponentInfo>;
            Assert.NotNull(response);
            Assert.Equal(1, response.Count);
        }
        [Fact]
        public async void CreatesComponentGivenCorrectModel(){
           
            string username = "Somebody";
            access.AddSuperadmin(username);
            ControllerTestUtility.SetCallersUsername(username, controller);

            ComponentInfo info = new ComponentInfo(0, "available", "comment");
            int expectedCreateId = repository.GetNextInsertId();
            
            CreatedResult result = await controller.CreateComponent(0, 1, info, username, "comment") as CreatedResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
            int? createdId = result.Value as int?;
            Assert.NotNull(createdId);
            Assert.Equal(expectedCreateId, createdId);
            Assert.True(repository.RepositoryContainsEntry((int)createdId));     
            Assert.Equal(await repository.GetComponent((int)createdId),info);
        }

        [Fact]
        public async void UpdatesComponentIfSuperUser(){
            
            const string username = "Somebody";
            const int newComponentTypeid = 101;
            ControllerTestUtility.SetCallersUsername(username, controller);
            ComponentInfo info = new ComponentInfo(1, "newStatus", "newComment");
            OkResult result = await controller.UpdateComponent(0, newComponentTypeid, info,"somebody", "comment") as OkResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

            ComponentInfo repositoryEntry = await repository.GetComponent(existingComponentId);
            Assert.NotNull(repositoryEntry);
            Assert.Equal(newComponentTypeid, repositoryEntry.id);
        }
        [Fact]
        public async void DeleteComponentIfSuperuser(){
            
            string username = "Somebody";
            access.AddSuperadmin(username);
            ControllerTestUtility.SetCallersUsername(username, controller);

            OkResult result = await controller.DeleteComponent(0, existingComponentId, "Somebody", "SletMig") as OkResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

            Assert.Null(await repository.GetComponent(existingComponentId));    
        }
        [Fact]
        public async void RollbackComponent(){
            //Findes den?
            //rigtige rank?
            //Er der noget at rollback til?
        }
    }
}