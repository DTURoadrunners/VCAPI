using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VCAPI.Filters;
using VCAPI.Repository;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;
using System.Linq;
using System.Collections.Generic;

namespace VCAPI.Controllers
{
    [Route("api/projects")]
    public class ProjectController : Controller
    {
        private readonly IProjectRepository repository;
        private readonly IResourceAccess resourceAccess;

        public ProjectController(IProjectRepository repo, IResourceAccess access){
            repository = repo;
            resourceAccess = access;
        }


        [HttpGet]
        public async Task<IActionResult> getAllProjects()
        {
            List<ProjectInfo> projs = await repository.GetProjects();
            if (projs != null)
            {
                return Ok(projs);
            }
            else
            {
                return BadRequest("No such project");
            }
        }

        [VerifyModelState]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> createProject([FromBody] ProjectInfo info, [FromBody]string comment)
        {
            if(!await resourceAccess.IsSuperAdmin(User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }
            
            int id = await repository.CreateProject(info.name, User.Identity.Name, comment);
            if(id == -1){
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
        public async Task<IActionResult> updateProject([FromRoute] int id, [FromBody] ProjectInfo model, [FromBody]string reason)
        {
            RANK rank = await resourceAccess.GetRankForProject(User.Identity.Name, id);
            if(rank < RANK.ADMIN){
                return Unauthorized();
            }

            if(!await repository.UpdateProject(model, id, User.Identity.Name, reason)){
                return BadRequest("Did not change project");
            }
            return Ok();
        }

        [HttpDelete("{projectId}")]
        [Authorize]
        public async Task<IActionResult> deleteProject([FromRoute] int id, [FromBody] string reason)
        {
            if(!await resourceAccess.IsSuperAdmin(User.Identity.Name))
                return Unauthorized();
            if(!await repository.DeleteProject(id, User.Identity.Name, reason)){
                return BadRequest("Project was not deleted");
            }
            return Ok();
        }
    }
}