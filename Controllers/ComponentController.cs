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
using System.Web.Http.Cors;

namespace VCAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Route("api/projects/{projectId}/componentType/{componentTypeId}/[controller]")]
    public class ComponentController : Controller
    {
        private readonly IComponentRepository repository;
        private readonly IResourceAccess resourceAccess;

        ///The constructor needs acces to a repo and the ability to work with it through IResource.
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

        ///C# preferably only wants one variable FromBody, so we use a MarshallObject
        public class ComponentMarshallObject
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
        [HttpDelete("{componentId}")]
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

        [HttpGet("{componentId}/revisions")]

        //Used to Rollback to a certain Id.
        public async Task<IActionResult> getRevisions([FromRoute] int componentId)
        {
            RevisionInfo[] revision = await repository.GetRevisions(componentId);
            return Ok(revision);
        }

        [Authorize]
        [HttpPut("{componentId}/rollback")]
        
        ///All Rollback functions requires a RollbackProjectMarshallObject,
        /// this contains the revisied id for which to revert to.
        public async Task<IActionResult> rollbackComponent([FromRoute] int projectId, [FromRoute] int componentId, [FromBody] RollbackProjectMarshallObject marshall)
        {
            string userId = User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value;
            if (await resourceAccess.GetRankForProject(userId, projectId) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }
            if (!await repository.RollbackComponent(componentId, marshall.revision, userId, marshall.comment))
            {
                return new BadRequestObjectResult("Failed to rollback log: " + marshall.revision);
            }

            return Ok();
        }

    }
}