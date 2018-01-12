using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VCAPI.Filters;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;
using System.Linq;
using System.Security.Claims;
using VCAPI.Repository;

namespace VCAPI.Controllers
{
    [Route("api/projects/{projectId}/componentType/{componentTypeId}/[controller]")]
    public class ComponentController : Controller
    {
        private readonly IComponentRepository repository;
        private readonly IResourceAccess resourceAccess;

        public ComponentController(IComponentRepository repository, IResourceAccess resourceAccess)
        {
            this.repository = repository;
            this.resourceAccess = resourceAccess;
        }

        [HttpGet("{componentId}")]
        public async Task<IActionResult> GetComponent([FromRoute]int componentId){
            ComponentInfo component = await repository.GetComponent(componentId);
            if (component != null)
            {
                return Ok(component);
            }
            else
            {
                return BadRequest("No such component");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetComponents([FromRoute]int projectId){
            List<ComponentInfo> components = await repository.GetComponents(projectId);
            if (components != null)
            {
                return Ok(components);
            }
            else
            {
                return BadRequest("No components under such componentType");
            }
        }

        public struct ComponentMarshallObject
        {
            public ComponentInfo model;
            public string comment;
        }

        [VerifyModelState]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateComponent([FromRoute] int projectId, [FromRoute] int componentTypeId, [FromBody] ComponentMarshallObject marshall)
        {
            string userId = User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value;
            if (await resourceAccess.GetRankForProject(userId, projectId) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }

            int id = await repository.CreateComponent(componentTypeId, marshall.model, userId, marshall.comment);
            if (id == -1)
            {
                return new BadRequestObjectResult("Failed to create component");
            }
            return Created("api/project/" + projectId + "/componentType/" + componentTypeId + "/component/" + id, null);
        }

        [Authorize]
        [HttpPut("{componentId}")]
        public async Task<IActionResult> UpdateComponent([FromRoute] int projectId, [FromRoute] int componentTypeId, [FromRoute]int componentId, [FromBody] ComponentMarshallObject marshall)
        {
            string userId = User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value;
            if (await resourceAccess.GetRankForProject(userId, projectId) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }

            marshall.model.id = componentId;

            if (!await repository.UpdateComponent(componentTypeId, marshall.model, userId, marshall.comment))
            {
                return new BadRequestObjectResult("Failed to update component: " + marshall.model.id);
            }

            return Ok();
        }

        [Authorize]
        [HttpPut("{componentId}")]
        public async Task<IActionResult> DeleteComponent([FromRoute] int projectId, [FromRoute] int componentId, [FromBody] string comment)
        {
            string userId = User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value;
            if (await resourceAccess.GetRankForProject(userId, projectId) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }

            if (!await repository.DeleteComponent(componentId, userId, comment))
            {
                return new BadRequestObjectResult("Failed to delete component: " + componentId);
            }

            return Ok();
        }

        [Authorize]
        [HttpPut("{componentId}")]
        public async Task<IActionResult> rollbackComponent([FromRoute] int projectId, [FromRoute] int logId, [FromBody] string userId, [FromBody] string comment)
        {
            if (await resourceAccess.GetRankForProject(User.Identity.Name, projectId) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }

            if (!await repository.RollbackComponent(logId, userId, comment))
            {
                return new BadRequestObjectResult("Failed to rollback log: " + logId);
            }

            return Ok();
        }

    }
}