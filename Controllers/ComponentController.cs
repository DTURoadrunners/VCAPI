using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VCAPI.Filters;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;

namespace VCAPI.Controllers
{
    [Route("api/project/{projectId}/componentType/{componentTypeId}/[controller]")]
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
        public async Task<IActionResult> GetComponents([FromRoute]int componentId){
            List<ComponentInfo> components = await repository.GetComponents(componentId);
            if (components != null)
            {
                return Ok(components);
            }
            else
            {
                return BadRequest("No components under such componentType");
            }
        }

        [VerifyModelState]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateComponent([FromRoute] int projectId, [FromRoute] int componentTypeId, [FromBody] ComponentInfo model, [FromBody] string userId, [FromBody] string comment)
        {
            if (await resourceAccess.GetRankForProject(User.Identity.Name, projectId) < Repository.RANK.STUDENT);
            {
                return Unauthorized();
            }

            int id = await repository.CreateComponent(componentTypeId, model, userId, comment);
            if (id != -1)
            {
                return new BadRequestObjectResult("Failed to create component");
            }
            return Created("api/project/" + projectId + "/componentType/" + componentTypeId + "/component/", id);
        }

        [Authorize]
        [HttpPut("{componentId}")]
        public async Task<IActionResult> UpdateComponent([FromRoute] int projectId, [FromRoute] int componentTypeId, [FromRoute] int componentId, [FromBody] ComponentInfo model, [FromBody] string userId, [FromBody] string comment)
        {
            if (await resourceAccess.GetRankForProject(User.Identity.Name, projectId) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpPut("{componentId}")]
        public async Task<IActionResult> DeleteComponent(){
            return Ok();
        }

    }
}