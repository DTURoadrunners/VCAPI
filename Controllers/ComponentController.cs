using Microsoft.AspNetCore.Mvc;

namespace VCAPI.Controllers
{
    [Route("api/componentType/{componentTypeId}/[controller]")]
    public class ComponentController : Controller
    {
        [HttpGet("{componentId}")]
        public IActionResult GetComponent([FromRoute]int componentId){
                return Ok(componentId);
        }

        [HttpGet]
        public IActionResult GetComponents(){
            return Ok();
        }

        [HttpPost]
        public IActionResult CreateComponent(){
            return Ok();
        }

        [HttpPut("{componentId}")]
        public IActionResult UpdateComponent(){
            return Ok();
        }

        [HttpPut("{componentId}")]
        public IActionResult DeleteComponent(){
            return Ok();
        }

    }
}