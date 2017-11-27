using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VCAPI.Filters;
using VCAPI.Repository;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;

namespace VCAPI.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        private readonly IProjectRepository repository;
        private readonly IResourceAccess resourceAccess;

        public ProjectController(IProjectRepository repo, IResourceAccess access){
            repository = repo;
            resourceAccess = access;
        }


        [HttpGet]
        public IActionResult getAllProjects()
        {
            return null;
        }

        [VerifyModelState]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> createProject([FromBody] ProjectInfo info)
        {
            if(!await resourceAccess.IsSuperAdmin(User.Identity.Name)){
                return Unauthorized();
            }
            
            int id = await repository.CreateProject(info);
            if(id == -1){
                // TODO: Return 500
                return new BadRequestObjectResult("Failed to create project");
            }
            else{
                return Created("/projects/", id);
            }
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> getProject([FromRoute] int id)
        {
            ProjectInfo proj = await repository.GetProject(id);
            if(proj != null){
                return Ok(proj);
            }
            else{
                return BadRequest("No such project");
            }
        }

        [Authorize]
        [HttpPut("{projectId}")]
        public async Task<IActionResult> updateProject([FromRoute] int id, [FromBody] ProjectInfo model)
        {
            RANK rank = await resourceAccess.GetRankForProject(User.Identity.Name, id);
            if(rank < RANK.ADMIN){
                return Unauthorized();
            }

            if(! await repository.UpdateProject(model, id)){
                return BadRequest("Did not change project");
            }
            return Ok();
        }

        [HttpDelete("{projectId}")]
        [Authorize]
        public async Task<IActionResult> deleteProject([FromRoute] int id)
        {
            if(!await resourceAccess.IsSuperAdmin(User.Identity.Name))
                return Unauthorized();
            if(!await repository.DeleteProject(id)){
                return BadRequest("Project was not deleted");
            }
            return Ok();
        }
    }
}