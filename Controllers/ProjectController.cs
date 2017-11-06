using Microsoft.AspNetCore.Mvc;


namespace VCAPI.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        
        [HttpGet]
        public IActionResult getAllProjects()
        {
            return null;
        }

        [HttpPost]
        public IActionResult createProject([FromBody] ProjectModel model)
        {
            return null;
        }

        [HttpGet("{id}")]
        public IActionResult readProject([FromRoute] int id)
        {
            return null;
        }

        [HttpPut("{id}")]
        public IActionResult updateProject([FromRoute] int id, [FromBody] ProjectModel model)
        {
            return null;
        }

        [HttpDelete("{id}")]
        public IActionResult deleteProject([FromRoute] int id)
        {
            return null;
        }
    }
}