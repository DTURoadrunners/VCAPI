using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VCAPI.Filters;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;
using VCAPI.Repository;
using System.Linq;
using System.Security.Claims;

namespace VCAPI.Controllers
{
    [Route("api/projects/{projectId}/[controller]")]
    public class ComponentTypeController : Controller
    {
        private readonly IComponentTypeRepository repository;
        private readonly IResourceAccess resourceAccess;

        public ComponentTypeController(IComponentTypeRepository repository, IResourceAccess resourceAccess)
        {
            this.repository = repository;
            this.resourceAccess = resourceAccess;
        }


        [HttpGet("{componentTypeId}")]
        public async Task<IActionResult> GetComponentType([FromRoute]int projectId, [FromRoute]int componentTypeId)
        {
            ComponentTypeInfo componentType = await repository.GetComponentType(componentTypeId, projectId);
            if (componentType != null)
            {
                return Ok(componentType);
            }
            else
            {
                return BadRequest("No such componenttype");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetComponentTypes([FromRoute]int projectId)
        {
            List<ComponentTypeInfo> componentTypes = await repository.GetComponentTypes(projectId);
            if (componentTypes != null)
            {
                return Ok(componentTypes);
            }
            else
            {
                return BadRequest("No componenttypes under this project");
            }
        }

        public class ComponentTypeMarshallObject
        {
            public ComponentTypeInfo model;
            public string comment;
        }

        [VerifyModelState]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateComponentType([FromRoute] int projectId, [FromBody] ComponentTypeMarshallObject marshall)
        {
            string userId = User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value;
            if (await resourceAccess.GetRankForProject(userId, 0) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }

            int id = await repository.CreateComponentType(marshall.model, projectId, userId, marshall.comment);
            if (id == -1)
            {
                return new BadRequestObjectResult("Failed to create componenttype");
            }
            return Created("api/project/" + projectId + "/componentType/" + id, null);
        }

        [Authorize]
        [VerifyModelState]
        [HttpPut("{componentTypeId}")]
        public async Task<IActionResult> UpdateComponentType([FromRoute] int projectId, [FromRoute] int componentTypeId, [FromBody] ComponentTypeMarshallObject marshall)
        {
            string userId = User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value;
            if (await resourceAccess.GetRankForProject(userId, 0) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }
            marshall.model.id = componentTypeId;
            if (!await repository.UpdateComponentType(marshall.model, userId, marshall.comment))
            {
                return new BadRequestObjectResult("Failed to update componenttype: " + marshall.model.id);
            }

            return Ok();
        }

        [Authorize]
        [VerifyModelState]
        [HttpDelete("{componentTypeId}")]
        public async Task<IActionResult> DeleteComponentType([FromRoute] int projectId, [FromRoute] int componentTypeId, [FromBody]string reason)
        {
            string userId = User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value;
            if (await resourceAccess.GetRankForProject(userId, 0) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }

            if (!await repository.DeleteComponentType(componentTypeId, userId, reason))
            {
                return new BadRequestObjectResult("Failed to delete componenttype: " + componentTypeId);
            }

            return Ok();
        }

        [Authorize]
        [VerifyModelState]
        [HttpPut("{componentTypeId}/rollback")]
        public async Task<IActionResult> rollbackComponentType([FromRoute] int projectId, [FromRoute] int logId, [FromBody] string userId, [FromBody] string comment)
        {
            if (await resourceAccess.GetRankForProject(User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value, 0) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }

            if (!await repository.RollbackComponentType(logId, userId, comment))
            {
                return new BadRequestObjectResult("Failed to rollback log: " + logId);
            }

            return Ok();
        }

    }
}