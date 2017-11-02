using Microsoft.AspNetCore.Mvc;

namespace VCAPI.Controllers
{
    [Route("api/[controller]")]
    public class ComponentTypeController : Controller
    {
        public class ComponentController : Controller
    {
        [HttpGet("{componentTypeId}")]
        public IActionResult GetComponent([FromRoute]int componentTypeId){
                return Ok(componentTypeId);
        }

        [HttpGet]
        public IActionResult GetComponents(){
            return Ok();
        }

        [HttpPost]
        public IActionResult CreateComponent(){
            return Ok();
        }

        [HttpPut("{componentTypeId}")]
        public IActionResult UpdateComponent(){
            return Ok();
        }

        [HttpPut("{componentTypeId}")]
        public IActionResult DeleteComponent(){
            return Ok();
        }

    }
    }
}