using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using VCAPI.Filters;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using System.Text;
using VCAPI.Repository.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using VCAPI.Options;

namespace VCAPI.Controllers
{
    [Route("api/")]
    public class LoginController : Controller
    {
        public struct RegisterCredentials
        {
            [Required]
            public string username { get; set; }
            [Required]
            public string password { get; set; }
            [Required]
            public string CASCODE { get; set; }
        }

        public class LoginCredentials
        {
            [Required]
            public string username { get; set; }
            [Required]
            public string password { get; set; }
        }

        private readonly Encoding encoding;
        private readonly IUserRepository repo;
        private readonly JWTOptions tokenSettings;
        private JwtSecurityTokenHandler jwtSecurityToken;
        private readonly byte[] symmetricKey;

        public LoginController(IUserRepository repo, IOptions<JWTOptions> settings)
        {
            tokenSettings = settings.Value;
            symmetricKey = Convert.FromBase64String(tokenSettings.secretKey);
            this.repo = repo;
            encoding = Encoding.UTF8;
            jwtSecurityToken = new JwtSecurityTokenHandler();
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

        [HttpPost("verify")]
        [Authorize]
        public IActionResult verify()
        {
            return Ok();
        }

        [HttpPut("login")]
        public IActionResult Login([FromBody]LoginCredentials credentials)
        {
            SecurityTokenDescriptor securityTokenRegister = new SecurityTokenDescriptor()
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Role, "User")
                }),
                Expires = DateTime.Now.AddHours(6),
                Audience = tokenSettings.audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = jwtSecurityToken.CreateToken(securityTokenRegister);
            String response = jwtSecurityToken.WriteToken(token);
            // Lookup user in DB
            return Ok(response);
        }
    }
}