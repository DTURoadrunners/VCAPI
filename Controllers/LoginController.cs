using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using VCAPI.Filters;
using System.Linq;
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
using VCAPI.Repository.Models;

using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Web.Http.Cors;

namespace VCAPI.Controllers
{
    [Route("api/")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : Controller
    {
        public struct RegisterCredentials
        {
            [Required]
            public string username { get; set; }
            [Required]
            public string firstname { get; set; }
            [Required]
            public string lastname { get; set; }
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
            
           /* HttpClient client = new HttpClient();
            HttpRequestMessage msg = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new System.Uri("http://www.campusnet.dtu.dk/data/CurrentUser/Userinfo")
            };
            msg.Headers.Add("x-appname", "Opslagsystem for �kobil");
            msg.Headers.Add("x-token", "3ddfc095-5a62-4162-a058-5bc3784e36d7");
            string token = Convert.ToBase64String(encoding.GetBytes(String.Format("{0}:{1}", credentials.username, credentials.CASCODE)));
            msg.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", token);
            HttpResponseMessage result = await client.SendAsync(msg);*/
            if (true /*result.IsSuccessStatusCode*/)
            {
                UserInfo i = new UserInfo{userID = credentials.username, firstname = credentials.firstname, lastname = credentials.lastname, password = Encoding.UTF8.GetBytes(credentials.password)};
                bool success = await repo.CreateUser(i);
                if(success)
                    return Ok();
                else
                    return BadRequest("User may already exists");
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
            return Ok("Hello, " + User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value);
        }

        [HttpPost("login")]
        public async  Task<IActionResult> Login([FromBody]LoginCredentials credentials)
        {

            if(await repo.Authenticate(credentials.username, credentials.password))
            {
                SecurityTokenDescriptor securityTokenRegister = new SecurityTokenDescriptor()
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, credentials.username)
                }),
                    Expires = DateTime.Now.AddHours(6),
                    Audience = tokenSettings.audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
                };
                SecurityToken token = jwtSecurityToken.CreateToken(securityTokenRegister);
                String response = jwtSecurityToken.WriteToken(token);
                JObject jObject = new JObject();
                jObject.Add("Token", response);
                return Ok(jObject);
            }

            return Unauthorized();
        }
    }
}