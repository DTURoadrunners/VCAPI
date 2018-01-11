using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using tests.MockRepository;
using tests.TestControllers;
using VCAPI.Controllers;
using VCAPI.Repository.Models;
using Xunit;

namespace VCAPI.Repository.ControllerTests
{
    public class ComponentTypeControllerTest
    {
        private readonly ComponentTypeMockRepository repository;
        private readonly ComponentTypeController controller;
        private readonly MockResourceAccess access;
        private int existingComponentTypeId;
        public ComponentTypeControllerTest()
        {
            repository = new ComponentTypeMockRepository();
            access = new MockResourceAccess();
            controller = new ComponentTypeController(repository, access);
            InitializeRepository();
        }
        private async void InitializeRepository()
        {
            existingComponentTypeId = await repository.CreateComponentType(
                new ComponentTypeInfo(1, "TestName", 1, 1, "Tests"), 1, "TestMcTest", "comment");
        }
        public async void GetComponentTypeReturnsValidsID(){
            OkObjectResult result = await controller.GetComponentType(existingComponentTypeId) as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

            ComponentTypeInfo response = result.Value as ComponentTypeInfo;
            Assert.NotNull(response);
        }
        [Fact]
        public async void GetComponentTypeReturnsBadRequest(){
            BadRequestObjectResult result = await controller.GetComponentType(existingComponentTypeId) as BadRequestObjectResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);

            ComponentTypeInfo response = result.Value as ComponentTypeInfo;
            Assert.NotNull(response);
        }
        [Fact]
        public async void ReturnsAllComponentTypes(){
            IActionResult result = await controller.GetComponentTypes(1);
            OkObjectResult okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
            List<ComponentTypeInfo> response = okResult.Value as List<ComponentTypeInfo>;
            Assert.NotNull(response);
            Assert.Equal(1, response.Count);
        }
        [Fact]
        public async void CreatesComponentTypeGivenCorrectModelAsSuper(){
           
            string username = "Somebody";
            access.AddSuperadmin(username);
            ControllerTestUtility.SetCallersUsername(username, controller);

            ComponentTypeInfo info = new ComponentTypeInfo(10, "TestTestTest", 5, 123, "TestComment");
            int expectedCreateId = repository.GetNextInsertId();

            CreatedResult result = await controller.CreateComponentType( 1, info, username, "comment") as CreatedResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
            int? createdId = result.Value as int?;
            Assert.NotNull(createdId);
            Assert.Equal(expectedCreateId, createdId);
            Assert.True(repository.RepositoryContainsEntry((int)createdId));     
            Assert.Equal(await repository.GetComponentType((int)createdId),info);  
        }
        public async void CreatesComponentTypeGivenCorrectModelAsStudent(){
           
            string anotherUsername = "SomebodyElse";
            access.AssignRankForProject(anotherUsername, 1, RANK.STUDENT);
            ControllerTestUtility.SetCallersUsername(anotherUsername, controller);
            
            ComponentTypeInfo info = new ComponentTypeInfo(10, "TestTestTest", 5, 123, "TestComment");
            int expectedCreateId = repository.GetNextInsertId();

            CreatedResult result = await controller.CreateComponentType( 1, info, anotherUsername, "comment") as CreatedResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
            int? createdId = result.Value as int?;
            Assert.NotNull(createdId);
            Assert.Equal(expectedCreateId, createdId);
            Assert.True(repository.RepositoryContainsEntry((int)createdId));     
            Assert.Equal(await repository.GetComponentType((int)createdId),info);
        }
        [Fact]
        public async void UpdatesComponentTypeIfSuperUser(){
             
            const string username = "Somebody";
            const int newComponentTypeid = 101;
            ControllerTestUtility.SetCallersUsername(username, controller);
            ComponentTypeInfo info = new ComponentTypeInfo(18, "TestTestTest", 12, 123, "TestComment");
            OkResult result = await controller.UpdateComponentType(0, info,"somebody", "comment") as OkResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

            ComponentTypeInfo repositoryEntry = await repository.GetComponentType(existingComponentTypeId);
            Assert.NotNull(repositoryEntry);
            Assert.Equal(newComponentTypeid, repositoryEntry.id);
        }
        [Fact]
        public async void DeleteComponentIfSuperuser(){
            
            string username = "Somebody";
            access.AddSuperadmin(username);
            ControllerTestUtility.SetCallersUsername(username, controller);

            OkResult result = await controller.DeleteComponentType(0, existingComponentTypeId, "Somebody", "SletMig") as OkResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

            Assert.Null(await repository.GetComponentType(existingComponentTypeId));     
        }
        [Fact]
        public async void RollbackComponent(){
        
        }
    }
}