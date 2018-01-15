using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VCAPI.Filters;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository;
using VCAPI.Repository.Models;
using System.Web.Http.Cors;

namespace VCAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Route("api/project/{projectId}/componentType/{componentTypeId}/[controller]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository repository;
        private readonly IResourceAccess resourceAccess;

        public CategoryController(ICategoryRepository repository, IResourceAccess resourceAccess)
        {
            this.repository = repository;
            this.resourceAccess = resourceAccess;
        }

        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetCategory([FromRoute]int categoryId){
            CategoryInfo category = await repository.GetCategory(categoryId);
            if (category != null)
            {
                return Ok(category);
            }
            else
            {
                return BadRequest("No such category");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromRoute]int projectId){
            List<CategoryInfo> categories = await repository.GetCategories(projectId);
            if (categories != null)
            {
                return Ok(categories);
            }
            else
            {
                return BadRequest("No categories in this project");
            }
        }

        [VerifyModelState]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCategories([FromRoute] int projectId, [FromBody] CategoryInfo model, [FromBody] string userId, [FromBody] string comment)
        {
            if (await resourceAccess.GetRankForProject(User.Identity.Name, projectId) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }

            int id = await repository.CreateCategory(projectId, model, userId, comment);
            if (id == -1)
            {
                return new BadRequestObjectResult("Failed to create category");
            }
            return Created("api/project/" + projectId + "/category", id);
        }

    }
}