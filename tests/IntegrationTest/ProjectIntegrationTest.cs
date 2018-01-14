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
using static VCAPI.Controllers.ComponentTypeController;

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
            Assert.True(createdId > 0);
            
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

            if(response.StatusCode == HttpStatusCode.OK)
            {
                byte[] data = await response.Content.ReadAsByteArrayAsync();
                string strData = Encoding.UTF8.GetString(data);
                return JsonConvert.DeserializeObject<ProjectInfo>(strData);
            }
            else
            {
                return null;
            }
        }

        private async Task<bool> DeleteProject(int projectId)
        {
            string jwtToken = await GetJWTToken(LoginCredentialProvider.GetSuperAdmin());
            SetAuthorization(client, jwtToken);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, "api/projects/" + projectId);
            string reason = "Deleted test proejct";
            request.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(reason)));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            return true;
        }   
        
        private async Task<RevisionInfo[]> GetProjectRevisions(int projectId)
        {
            string jwtToken = await GetJWTToken(LoginCredentialProvider.GetSuperAdmin());
            SetAuthorization(client, jwtToken);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/projects/" + projectId + "/revisions");
            HttpResponseMessage response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            RevisionInfo[] revisons = JsonConvert.DeserializeObject<RevisionInfo[]>(await response.Content.ReadAsStringAsync());
            return revisons;
        }

        private async Task<bool> RollbackProjectRevision(int projectId, int revision)
        {
            ProjectController.RollbackProjectMarshallObject marshallObject = new ProjectController.RollbackProjectMarshallObject()
            {
                comment = "Test rollback",
                revision = revision
            };
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, "api/projects/" + projectId + "/rollback");
            request.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(marshallObject)));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return true;
        }
        
        [Fact]
        public async void CheckProjectEndpointCRUD()
        {
            const string originalName = "TestProject1";
            ProjectInfo newProject = new ProjectInfo(0, originalName);
            
            int createdId = await TestCreateProject(newProject);
            newProject.id = createdId;
            newProject.name = "NewName";
            await TestUpdateProject(newProject);
            ProjectInfo info = await TestGetProject(createdId);
            Assert.Equal(newProject.name, info.name);
            Assert.Equal(info.id, createdId);

            await DeleteProject(createdId);
            Assert.Null(await TestGetProject(createdId));

            RevisionInfo[] revisons = await GetProjectRevisions(createdId);
            Assert.Equal("created", revisons[2].eventType);
            await RollbackProjectRevision(createdId, revisons[2].revisonId);
            info = await TestGetProject(createdId);
            Assert.NotNull(info);
            Assert.Equal(originalName.ToLower(), info.name);
            revisons = await GetProjectRevisions(createdId);
            Assert.Equal("rollback", revisons[0].eventType);
        }

        [Fact]
        public async void CheckComponentTypeEndpointCRUD()
        {
            const string updatedLabel = "Updated component type"; 
            ComponentTypeInfo componentType = new ComponentTypeInfo(0, "Test Component", 1, 1, "Test Component");
            int componentTypeId = await CreateNewComponentType(componentType);
            componentType.name = updatedLabel;
            await UpdateComponentType(componentTypeId, componentType);
            ComponentTypeInfo updatedComponent = await GetComponentType(componentTypeId);
            Assert.Equal(updatedLabel, updatedComponent.name);
            await DeleteComponentType(componentTypeId);
            ComponentTypeInfo expectedNull = await GetComponentType(componentTypeId);
            Assert.Null(expectedNull);
        }

        private async Task<ComponentTypeInfo> GetComponentType(int componentTypeId)
        {
            string jwtToken = await GetJWTToken(LoginCredentialProvider.GetSuperAdmin());
            SetAuthorization(client, jwtToken);
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "api/projects/1/componentType/" + componentTypeId);
            HttpResponseMessage response = await client.SendAsync(message);
            if(response.StatusCode == HttpStatusCode.OK)
            {
                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ComponentTypeInfo>(content);
            }
            else
            {
                return null;
            }
        }

        private async Task<bool> DeleteComponentType(int idToDelete)
        {
            string jwtToken = await GetJWTToken(LoginCredentialProvider.GetSuperAdmin());
            SetAuthorization(client, jwtToken);
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, "api/projects/1/componentType/" + idToDelete);
            string reason = "deleted test component";
            message.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(reason)));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            return true;
        }

        private async Task<bool> UpdateComponentType(int idToUpdate, ComponentTypeInfo newObject)
        {
            string jwtToken = await GetJWTToken(LoginCredentialProvider.GetSuperAdmin());
            SetAuthorization(client, jwtToken);
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, "api/projects/1/componentType/" + idToUpdate);
            ComponentTypeMarshallObject marshall = new ComponentTypeMarshallObject()
            {
                model = newObject,
                comment = "Update project"
            };
            message.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(marshall)));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            return true;
        }

        private async Task<int> CreateNewComponentType(ComponentTypeInfo newObject)
        {
            string jwtToken = await GetJWTToken(LoginCredentialProvider.GetSuperAdmin());
            SetAuthorization(client, jwtToken);
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "api/projects/1/componentType");
            ComponentTypeMarshallObject marshall = new ComponentTypeMarshallObject(){
                model = newObject,
                comment = "Test Project"
            };
            message.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(marshall)));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            int createdId = GetCreatedId(response.Headers.Location.ToString());
            Assert.True(createdId > 0);
            
            return createdId;
        }

        [Fact]
        public async void CheckComponentEndpointCRUD()
        {
            ComponentInfo componentType = new ComponentInfo(0, "In use", "Test Component");
            int componentId = await CreateComponent(componentType);
            Assert.True(componentId > 0);
            componentType.status = "Test update";
            await UpdateComponent(componentId, componentType);
            ComponentInfo updatedComponentType = await GetComponent(componentId);
            Assert.Equal(updatedComponentType.comment, componentType.comment);
            Assert.Equal(updatedComponentType.status, componentType.status);
            Assert.Equal(updatedComponentType.id, componentId);
            await DeleteComponent(componentId);
            ComponentInfo expectedNull = await GetComponent(componentId);
            Assert.Null(expectedNull);
        }

        private async Task<int> CreateComponent(ComponentInfo info)
        {
            string jwtToken = await GetJWTToken(LoginCredentialProvider.GetSuperAdmin());
            SetAuthorization(client, jwtToken);
            ComponentController.ComponentMarshallObject content = new ComponentController.ComponentMarshallObject(){
                model = info,
                comment = "Create test object"
            };
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/projects/1/componentType/1/component");
            request.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(content)));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return GetCreatedId(response.Headers.Location.ToString());
        }

        private async Task<bool> UpdateComponent(int IdToUpdate, ComponentInfo newInfo)
        {
            string jwtToken = await GetJWTToken(LoginCredentialProvider.GetSuperAdmin());
            SetAuthorization(client, jwtToken);
            ComponentController.ComponentMarshallObject content = new ComponentController.ComponentMarshallObject(){
                model = newInfo,
                comment = "Update test object"
            };
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, "api/projects/1/componentType/1/component/" + IdToUpdate);
            request.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(content)));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            HttpResponseMessage response = await client.SendAsync(request);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            return true;
        }

        private async Task<ComponentInfo> GetComponent(int idToUpdate)
        {
            string jwtToken = await GetJWTToken(LoginCredentialProvider.GetSuperAdmin());
            SetAuthorization(client, jwtToken);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/projects/1/componentType/1/component/" + idToUpdate);
            
            HttpResponseMessage response = await client.SendAsync(request);
            if(response.StatusCode == HttpStatusCode.OK)
            {
                byte[] reply = await response.Content.ReadAsByteArrayAsync();
                return JsonConvert.DeserializeObject<ComponentInfo>(Encoding.UTF8.GetString(reply));
            }
            else
            {
                return null;
            }
        }

        private async Task<bool> DeleteComponent(int idToDelete)
        {
            string jwtToken = await GetJWTToken(LoginCredentialProvider.GetSuperAdmin());
            SetAuthorization(client, jwtToken);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, "api/projects/1/componentType/1/component/" + idToDelete);
            string reason = "deleted test component";
            request.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(reason)));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            return true;
        }

    }
}