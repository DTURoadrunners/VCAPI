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
            BadRequestObjectResult result = await controller.GetComponent(100) as BadRequestObjectResult;
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
        public async void CreatesComponentGivenCorrectModelAsSuper(){
           
            string username = "Somebody";
            access.AddSuperadmin(username);
            ControllerTestUtility.SetCallersUsername(username, controller);

            int expectedCreateId = repository.GetNextInsertId();
            ComponentController.ComponentMarshallObject marshall = new ComponentController.ComponentMarshallObject();

            marshall.model = new ComponentInfo(0, "available", "comment");
            marshall.comment = "initialize create";
            
            ActionResult controllerResponse = await controller.CreateComponent(0, 1, marshall) as ActionResult;
            Assert.Equal(typeof(CreatedResult), controllerResponse.GetType());
            CreatedResult result = controllerResponse as CreatedResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
            string createdIdStr = result.Location.Substring(result.Location.LastIndexOf('/') + 1);
            int createdId = int.Parse(createdIdStr);
            Assert.Equal(expectedCreateId, createdId);
            Assert.True(repository.RepositoryContainsEntry((int)createdId));     
            Assert.Equal(await repository.GetComponent((int)createdId),marshall.model);
        }
        public async void CreatesComponentGivenCorrectModelAsStudent(){
           
            string anotherUsername = "SomebodyElse";
            access.AssignRankForProject(anotherUsername, 1, RANK.STUDENT);
            ControllerTestUtility.SetCallersUsername(anotherUsername, controller);

            int expectedCreateId = repository.GetNextInsertId();
            ComponentController.ComponentMarshallObject marshall = new ComponentController.ComponentMarshallObject();

            marshall.model = new ComponentInfo(0, "available", "comment");
            marshall.comment = "initialize create";
            
            ActionResult controllerResponse = await controller.CreateComponent(0, 1, marshall) as ActionResult;
            Assert.Equal(typeof(CreatedResult), controllerResponse.GetType());
            CreatedResult result = controllerResponse as CreatedResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
            string createdIdStr = result.Location.Substring(result.Location.LastIndexOf('/') + 1);
            int createdId = int.Parse(createdIdStr);
            Assert.Equal(expectedCreateId, createdId);
            Assert.True(repository.RepositoryContainsEntry((int)createdId));     
            Assert.Equal(await repository.GetComponent((int)createdId),marshall.model);
        }

        [Fact]
        public async void UpdatesComponentIfSuperUser(){
            
            const string username = "Somebody";
            access.AddSuperadmin(username);
            const int Componentid = 0;
            ControllerTestUtility.SetCallersUsername(username, controller);
            ComponentController.ComponentMarshallObject marshall = new ComponentController.ComponentMarshallObject();

            marshall.model = new ComponentInfo(0, "not available", "comment");
            marshall.comment = "initialize update";
            OkResult result = await controller.UpdateComponent(0, Componentid, 10, marshall) as OkResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

            ComponentInfo repositoryEntry = await repository.GetComponent(existingComponentId);
            Assert.NotNull(repositoryEntry);
            Assert.Equal(10, repositoryEntry.id);
        }
        [Fact]
        public async void DeleteComponentIfSuperuser(){
            
            string username = "Somebody";
            access.AddSuperadmin(username);
            ControllerTestUtility.SetCallersUsername(username, controller);

            OkResult result = await controller.DeleteComponent(0, existingComponentId, "deleteThis") as OkResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

            Assert.Null(await repository.GetComponent(existingComponentId));    
        }
        [Fact]
        public async void RollbackComponent(){
            
        }
    }
}