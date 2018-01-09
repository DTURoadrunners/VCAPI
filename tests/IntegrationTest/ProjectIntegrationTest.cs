using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using Microsoft.Extensions.Configuration;
using static VCAPI.Controllers.LoginController;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.AspNetCore;
using Newtonsoft.Json.Linq;
using VCAPI.Repository.Models;

namespace tests.IntegrationTest
{
    public static class JObjectExtensions
    {
        public static byte[] ToByteArray(this JObject obj)
        {
            return Encoding.UTF8.GetBytes(obj.ToString());
        }
    }
    
    public class ProjectIntegrationTest
    {
        private readonly TestServer testServer;
        private readonly HttpClient client;
        public ProjectIntegrationTest()
        {
            testServer = new TestServer(WebHost.CreateDefaultBuilder().
            ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("TestSettings.json");
            }
            ).UseStartup<VCAPI.Startup>());
            client = testServer.CreateClient();
        }

        private static void SetAuthorization(HttpClient client, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<string> GetJWTToken(LoginCredentials credentials)
        {
            string strContent = JsonConvert.SerializeObject(credentials);
            byte[] content = Encoding.UTF8.GetBytes(strContent);
            HttpContent payload = new ByteArrayContent(content);
            payload.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            var response = await client.PutAsync("api/login", payload);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(); 
        }

        private async void VerifyJWTToken(string token)
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "api/verify");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(message);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void CanLoginUser()
        {
            LoginCredentials credentials = LoginCredentialProvider.GetSuperAdmin();
            string jwt = await GetJWTToken(credentials);
            Assert.NotNull(jwt);
            Assert.True(jwt.Length > 0);
            VerifyJWTToken(jwt);
        }

        [Fact]
        public async void CheckProjectEndpointCRUD()
        {
            JObject jObject = new JObject();
            ProjectInfo newProject = new ProjectInfo(0, "TestProject");
            jObject["info"] = JObject.FromObject(newProject);
            jObject["comment"] = "For testing purposes";
            string strBody = jObject.ToString();
            string jwtToken = await GetJWTToken(LoginCredentialProvider.GetSuperAdmin());
            SetAuthorization(client, jwtToken);

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "api/projects");
            message.Content = new ByteArrayContent(jObject.ToByteArray());
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            HttpResponseMessage response = await client.SendAsync(message);
            response.EnsureSuccessStatusCode();
        }

    }
}