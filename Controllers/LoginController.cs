using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace VCAPI.Controllers
{
    [Route("api/")]
    public class LoginController : Controller
    {
        public class RegisterCredentials{
            [Required]
            [MinLength(1)]
            public string username;
            [Required]
            public string password;
            [Required]
            public string CASCODE;
        }

        public class LoginCredentials{
            [Required]
            public string username;
            [Required]
            public string password;
        }

        [HttpPost("signup")]
        public IActionResult Signup([FromBody] RegisterCredentials credentials){
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values);
            }
            return Ok(credentials);
        }

        [HttpPut("login")]
        public IActionResult Login(){
            return Ok();
        }
    }
}