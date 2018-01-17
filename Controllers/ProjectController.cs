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
using System.Web.Http.Cors;

namespace VCAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Route("api/projects")]
    public class ProjectController : Controller
    {
        private readonly IProjectRepository repository;
        private readonly IResourceAccess resourceAccess;
        private string authorizedUser
        {
            get
            {
                return User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value;
            }
        }
        ///The constructor needs acces to a repo and the ability to work with it through IResource.
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

        public struct CreateProjectMarshall
        {
            public ProjectInfo info;
            public string comment;
        }

        [VerifyModelState]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> createProject([FromBody] CreateProjectMarshall request)
        { 
            if(!await resourceAccess.IsSuperAdmin(authorizedUser)){
                return Unauthorized();
            }
            
            int id = await repository.CreateProject(request.info.name, authorizedUser, request.comment);
            if(id == -1){
                return new BadRequestObjectResult("Failed to create project");
            }
            else{
                return Created("/projects/" + id, null);
            }
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> getProject([FromRoute] int projectId)
        {
            ProjectInfo proj = await repository.GetProject(projectId);
            if(proj != null){
                return Ok(proj);
            }
            else{
                return BadRequest("No such project");
            }
        }

        [Authorize]
        [HttpPut("{projectId}")]
        public async Task<IActionResult> updateProject([FromRoute] int projectId, [FromBody] CreateProjectMarshall marshall)
        {
            RANK rank = await resourceAccess.GetRankForProject(authorizedUser, projectId);
            if(rank < RANK.ADMIN){
                return Unauthorized();
            }

            if(!await repository.UpdateProject(marshall.info, projectId, authorizedUser, marshall.comment)){
                return BadRequest("Did not change project");
            }
            return Ok();
        }

        [HttpDelete("{projectId}")]
        [Authorize]
        public async Task<IActionResult> deleteProject([FromRoute] int projectId, [FromBody] string reason)
        {
            if (!await resourceAccess.IsSuperAdmin(authorizedUser))
                return Unauthorized();
            if(!await repository.DeleteProject(projectId, authorizedUser, reason)){
                return BadRequest("Project was not deleted");
            }
            return Ok();
        }

        [HttpGet("{projectId}/revisions")]
        public async Task<IActionResult> getRevisions([FromRoute]int projectId)
        {
            RevisionInfo[] revisions = await repository.GetRevisions(projectId);
            return Ok(revisions);
        }

        [HttpPut("{projectId}/rollback")]
        [VerifyModelState]
        [Authorize]

        ///All Rollback functions requires a RollbackProjectMarshallObject,
        ///this contains the revisied id for which to revert to.
        public async Task<IActionResult> rollbackProject([FromRoute] int projectId, [FromBody]RollbackProjectMarshallObject marshall)
        {
            if (!await resourceAccess.IsSuperAdmin(authorizedUser))
                return Unauthorized();

            if (!await repository.RollbackProject(projectId, marshall.revision, authorizedUser, marshall.comment))
            {
                return BadRequest("Failed to rollback to revision");
            }

            return Ok();
        }
    }
}