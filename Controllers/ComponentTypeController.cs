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
    [Route("api/project/{projectId}/[controller]")]
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
        public async Task<IActionResult> GetComponentType([FromRoute]int componentTypeId)
        {
            ComponentTypeInfo componentType = await repository.GetComponentType(componentTypeId);
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

        [VerifyModelState]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateComponentType([FromRoute] int projectId, [FromBody] ComponentTypeInfo model, [FromBody] string userId, [FromBody] string comment)
        {
            if (await resourceAccess.GetRankForProject(User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value, 0) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }

            int id = await repository.CreateComponentType(model, projectId, userId, comment);
            if (id == -1)
            {
                return new BadRequestObjectResult("Failed to create componenttype");
            }
            return Created("api/project/" + projectId + "/componentType/", id);
        }

        [Authorize]
        [HttpPut("{componentTypeId}")]
        public async Task<IActionResult> UpdateComponentType([FromRoute] int projectId, [FromBody] ComponentTypeInfo model, [FromBody] string userId, [FromBody] string comment)
        {
            if (await resourceAccess.GetRankForProject(User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value, 0) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }

            if (!await repository.UpdateComponentType(model, projectId, userId, comment))
            {
                return new BadRequestObjectResult("Failed to update componenttype: " + model.id);
            }

            return Ok();
        }

        [Authorize]
        [HttpPut("{componentTypeId}")]
        public async Task<IActionResult> DeleteComponentType([FromRoute] int projectId, [FromRoute] int componentTypeId, [FromBody] string userId, [FromBody] string comment)
        {
            if (await resourceAccess.GetRankForProject(User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value, 0) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }

            if (!await repository.DeleteComponentType(componentTypeId, userId, comment))
            {
                return new BadRequestObjectResult("Failed to delete componenttype: " + componentTypeId);
            }

            return Ok();
        }

        [Authorize]
        [HttpPut("{componentTypeId}")]
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