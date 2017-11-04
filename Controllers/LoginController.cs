using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using VCAPI.Filters;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using System.Text;
using VCAPI.Repository.Interfaces;

namespace VCAPI.Controllers
{
    [Route("api/")]
    public class LoginController : Controller
    {
        private readonly Encoding encoding;
        private readonly IUserRepository repo;

        public LoginController(IUserRepository repo)
        {
            this.repo = repo;
            encoding = Encoding.UTF8; 
        }

        public struct RegisterCredentials{
            [Required]
            public string username { get; set; }
            [Required]
            public string password { get; set; }
            [Required]
            public string CASCODE { get; set; }
        }

        public class LoginCredentials{
            [Required]
            public string username { get; set; }
            [Required]
            public string password { get; set; }
        }

        [HttpPost("signup")]
        [VerifyModelState]
        public async Task<IActionResult> Signup([FromBody] RegisterCredentials credentials){
            HttpClient client = new HttpClient();
            HttpRequestMessage msg = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new System.Uri("http://www.campusnet.dtu.dk/data/CurrentUser/Userinfo")
            };
            msg.Headers.Add("x-appname", "Opslagsystem for økobil");
            msg.Headers.Add("x-token", "3ddfc095-5a62-4162-a058-5bc3784e36d7");
            string token = Convert.ToBase64String(encoding.GetBytes(String.Format("{0}:{1}", credentials.username, credentials.CASCODE)));
          //  string token = encoding.GetString(Convert.FromBase64String(String.Format("{0}:{1}", credentials.username, credentials.CASCODE)));
            msg.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", token);
            HttpResponseMessage result = await client.SendAsync(msg);
            if (result.IsSuccessStatusCode)
            {
                // Create user
                return Ok();
            }
            else
            {
                return BadRequest("Failed to verify login with campusnet");
            }

        }

        [HttpPut("login")]
        public IActionResult Login([FromBody]LoginCredentials credentials)
        {
            // Lookup user in DB
            return Ok();
        }
    }
}