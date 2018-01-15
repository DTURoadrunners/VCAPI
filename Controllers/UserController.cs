using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using VCAPI.Filters;
using VCAPI.Repository;
using System.Web.Http.Cors;

namespace VCAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Produces("application/json")]
    [Route("api/projects/{projectID}/UserPermissions")]
    public class UserController : Controller
    {
        private readonly IUserRepository repository;
        private readonly IResourceAccess resourceAccess;

        public UserController(IUserRepository repo, IResourceAccess access)
        {
            repository = repo;
            resourceAccess = access;
        }


        // GET: api/User/5
        [HttpGet("{id}")]
        [VerifyModelState]
        [Authorize]
        public async Task<IActionResult> GetUserInfo([FromRoute]string id, [FromRoute]int projectID)
        {
            string username = User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value;
            UserInfo requester = await repository.GetUser(username);
            RANK rank = await resourceAccess.GetRankForProject(username, projectID);
            if (!requester.superuser && rank < RANK.ADMIN)
            {
                return Unauthorized();
            }

            UserInfo info = await repository.GetUser(id);
            if(info == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(info);
            }
        }
              
        // PUT: api/User/5
        [HttpPut("{id}")]
        [VerifyModelState]
        [Authorize]
        public async Task<IActionResult> Put([FromRoute]string id, [FromBody]RANK rank, [FromRoute]int projectID)
        {
            string username = User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value;
            UserInfo requester = await repository.GetUser(username);
            RANK requesterRank = await resourceAccess.GetRankForProject(username, projectID);
            if (!requester.superuser && requesterRank < RANK.ADMIN)
            {
                return Unauthorized();
            }

            UserInfo info = await repository.GetUser(id);
            if (info == null)
            {
                return NotFound();
            }
            else
            {
                bool success = await resourceAccess.AssignRankForProject(id, projectID, rank);
                if (success)
                    return Ok(info);
                else
                    return BadRequest("Try again later");
            }
        }
    }
}
