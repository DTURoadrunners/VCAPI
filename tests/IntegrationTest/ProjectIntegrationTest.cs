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
using System.Net;
using VCAPI.Controllers;

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
        public async void CanRegisterUser()
        {
            RegisterCredentials registerObject = new RegisterCredentials()
            {
                username = "Hello",
                firstname = "Test",
                lastname = "User",
                password = "Sesame",
                CASCODE = "Ignored"
            };
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "api/signup");
            message.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(registerObject)));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
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

        private int GetCreatedId(string path)
        {
            int terminator = path.LastIndexOf('/');
            string strId = path.Substring(terminator + 1);
            return int.Parse(strId);
        }

        private async Task<int> TestCreateProject(ProjectInfo objectToCreate)
        {
            // Shady business
            ProjectController.CreateProjectMarshall marshall = new ProjectController.CreateProjectMarshall();
            marshall.info = objectToCreate;
            marshall.comment = "Created Test Project";

            string strBody = JsonConvert.SerializeObject(marshall);
            
            string jwtToken = await GetJWTToken(LoginCredentialProvider.GetSuperAdmin());
            SetAuthorization(client, jwtToken);

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "api/projects");
            message.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(strBody));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            HttpResponseMessage response = await client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);

            int createdId = GetCreatedId(response.Headers.Location.ToString());
            Assert.True(createdId >= 0);
            
            return createdId;
        }

        private async Task TestUpdateProject(ProjectInfo objectToUpdate)
        {
            ProjectController.CreateProjectMarshall marshall = new ProjectController.CreateProjectMarshall();
            marshall.info = objectToUpdate;
            marshall.comment = "Test update";
            string strBody = JsonConvert.SerializeObject(marshall);
            string jwtToken = await GetJWTToken(LoginCredentialProvider.GetSuperAdmin());
            SetAuthorization(client, jwtToken);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, "api/projects/" + objectToUpdate.id);
            request.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(strBody));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<ProjectInfo> TestGetProject(int id)
        {
            string jwtToken = await GetJWTToken(LoginCredentialProvider.GetSuperAdmin());
            SetAuthorization(client, jwtToken);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/projects/"+id);
            HttpResponseMessage response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            byte[] data = await response.Content.ReadAsByteArrayAsync();
            string strData = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<ProjectInfo>(strData);
        }

        [Fact]
        public async void CheckProjectEndpointCRUD()
        {
            ProjectInfo newProject = new ProjectInfo(0, "TestProject1");
            
            int createdId = await TestCreateProject(newProject);
            newProject.id = createdId;
            newProject.name = "NewName";
            await TestUpdateProject(newProject);
            ProjectInfo info = await TestGetProject(createdId);
            Assert.Equal(newProject.name, info.name);
        }

        [Fact]
        public async void CheckComponentTypeEndpointCRUD()
        {
            string jwtToken = await GetJWTToken(LoginCredentialProvider.GetSuperAdmin());
            SetAuthorization(client, jwtToken);
            string strBody = "";
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "api/projects/1/componentType");
            message.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(strBody));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        [Fact]
        public async void CheckComponentEndpointCRUD()
        {

        }
    }
}