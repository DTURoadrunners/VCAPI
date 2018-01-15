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
            OkObjectResult result = await controller.GetComponentType(1, existingComponentTypeId) as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

            ComponentTypeInfo response = result.Value as ComponentTypeInfo;
            Assert.NotNull(response);

            //TODO: Check også at response rent faktisk indeholder de samme fælter som er i mock databasen
        }
        [Fact]
        public async void GetComponentTypeReturnsBadRequest(){
            BadRequestObjectResult result = await controller.GetComponentType(2, 4) as BadRequestObjectResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
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
            ComponentTypeController.ComponentTypeMarshallObject input = new ComponentTypeController.ComponentTypeMarshallObject();        
            ComponentTypeInfo info = new ComponentTypeInfo(10, "TestTestTest", 5, 123, "TestComment");
            input.model = info;
            input.comment = "Test component type";
            int expectedCreateId = repository.GetNextInsertId();

            CreatedResult result = await controller.CreateComponentType(1, input) as CreatedResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
            int? createdId = result.Value as int?;
            Assert.NotNull(createdId);
            Assert.Equal(expectedCreateId, createdId);
            Assert.True(repository.RepositoryContainsEntry((int)createdId));     
            Assert.Equal(await repository.GetComponentType((int)createdId, 1),info);  
        }
        public async void CreatesComponentTypeGivenCorrectModelAsStudent(){
           
            string anotherUsername = "SomebodyElse";
            access.AssignRankForProject(anotherUsername, 1, RANK.STUDENT);
            ControllerTestUtility.SetCallersUsername(anotherUsername, controller);
    
            ComponentTypeController.ComponentTypeMarshallObject input = new ComponentTypeController.ComponentTypeMarshallObject();        
            ComponentTypeInfo info = new ComponentTypeInfo(10, "TestTestTest", 5, 123, "TestDescription");
            input.model = info;
            input.comment = "A test component type";
            
            int expectedCreateId = repository.GetNextInsertId();

            CreatedResult result = await controller.CreateComponentType(1, input) as CreatedResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
            int? createdId = result.Value as int?;
            Assert.NotNull(createdId);
            Assert.Equal(expectedCreateId, createdId);
            Assert.True(repository.RepositoryContainsEntry((int)createdId));     
            Assert.Equal(await repository.GetComponentType((int)createdId, 1),info);
        }
        [Fact]
        public async void UpdatesComponentTypeIfSuperUser(){
             
            const string username = "Somebody";
            access.AddSuperadmin(username);
            ControllerTestUtility.SetCallersUsername(username, controller);
            ComponentTypeController.ComponentTypeMarshallObject input = new ComponentTypeController.ComponentTypeMarshallObject();        
            ComponentTypeInfo info = new ComponentTypeInfo(1, "TestTestTest", 1, 123, "TestComment");
            input.model = info;
            input.comment = "Test component type";
            OkResult result = await controller.UpdateComponentType(0, existingComponentTypeId, input) as OkResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

            ComponentTypeInfo repositoryEntry = await repository.GetComponentType(existingComponentTypeId, 1);
            Assert.NotNull(repositoryEntry);
            Assert.Equal(0, repositoryEntry.id);
        }
        [Fact]
        public async void DeleteComponentTypeIfSuperuser(){
            
            string username = "Somebody";
            access.AddSuperadmin(username);
            ControllerTestUtility.SetCallersUsername(username, controller);

            OkResult result = await controller.DeleteComponentType(0, existingComponentTypeId, "SletMig") as OkResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

            Assert.Null(await repository.GetComponentType(existingComponentTypeId, 1));     
        }
        [Fact]
        public async void RollbackComponentTypeAsStudent(){
            string username = "Somebody1";
            access.AssignRankForProject(username, 1, RANK.STUDENT);
            ControllerTestUtility.SetCallersUsername(username, controller);


        }

        public async void RollbackComponentTypeAsGuest(){
            string anotherUsername = "SomebodyElse1";
            access.AssignRankForProject(anotherUsername, 1, RANK.GUEST);
            ControllerTestUtility.SetCallersUsername(anotherUsername, controller);
        }
    }
}