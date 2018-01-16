using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VCAPI.Filters;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;
using VCAPI.Repository;
using System.Linq;
using System.Security.Claims;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;

namespace VCAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Route("api/project/{projectId}/componentType/{componentTypeId}/[controller]")]
    public class DocumentController : Controller
    {
        private readonly IDocumentRepository repository;
        private readonly IResourceAccess resourceAccess;
        private readonly string dataPath;
        private string authorizedUser
        {
            get
            {
                return User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value;
            }
        }
        public DocumentController(IDocumentRepository repository, IResourceAccess resourceAccess, IHostingEnvironment env)
        {
            this.repository = repository;
            this.resourceAccess = resourceAccess;
            dataPath = env.ContentRootPath + "/data/";
        }


        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetDocument([FromRoute]int documentId)
        {
            DocumentInfo document = await repository.GetDocument(documentId);
            if (document != null)
            {
                // The requestor has no use for bucket path
                document.bucketpath = null;
                return Ok(document);
            }
            else
            {
                return BadRequest("No such document");
            }
        }

        [HttpGet("{documentId}/data")]
        public async Task<IActionResult> DownloadFile([FromRoute] int projectId, [FromRoute]int documentId)
        {
            if (await resourceAccess.GetRankForProject(authorizedUser, projectId) < Repository.RANK.STUDENT)    
            {
                return Unauthorized();
            }


            DocumentInfo document = await repository.GetDocument(documentId);
            if(document == null)
            {
                return NotFound();
            }

            string filePath = dataPath + document.bucketpath;
            FileStream readStream;
            try
            {
                readStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            }
            catch(Exception e)
            {
                return new ObjectResult(e.Message){
                        StatusCode = 500
                };
            }
            byte[] data = new byte[readStream.Length];
            await readStream.ReadAsync(data, 0, (int)readStream.Length);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetDocuments([FromRoute]int componentTypeId)
        {
            List<DocumentInfo> documents = await repository.getDocuments(componentTypeId);
            if (documents != null && documents.Count > 0)
            {
                return Ok(documents);
            }
            else
            {
                return BadRequest("No documents under such componentType");
            }
        }

        [VerifyModelState]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateDocument([FromRoute] int projectId, [FromRoute] int componentTypeId, IFormFile file)
        {
            if (await resourceAccess.GetRankForProject(authorizedUser, projectId) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }
            if(file == null)
            {
                return BadRequest("No file supplied or file with invalid key supplied. Please supply multipart file using key \"file\"");
            }
            StringValues description;
            Request.Form.TryGetValue("description", out description);
            DocumentInfo info = new DocumentInfo()
            {   
                filename = file.FileName,
                description = description.FirstOrDefault()
            };

            //TODO: Make this act as a "transaction"
            string bucketName = await StoreToDisc(file);
            StringValues comment;
            Request.Form.TryGetValue("comment", out comment);
            info.bucketpath = bucketName;

            int id = await repository.CreateDocument(info, authorizedUser, comment.FirstOrDefault(), componentTypeId);
            if (id == -1)
            {
                return new BadRequestObjectResult("Failed to create component");
            }
            return Created("api/project/" + projectId + "/componentType/" + componentTypeId + "/document/" + id, null);
        }

        private async Task<string> StoreToDisc(IFormFile file)
        {
            string tmpPath = Path.GetTempFileName(); 
            FileStream outStream = new FileStream(tmpPath, FileMode.Create);
            await file.CopyToAsync(outStream);
            await outStream.FlushAsync();
            outStream.Close();
            string filename;
            string path;
            do{
                filename = Path.GetRandomFileName();
                path =  dataPath + filename;
            // There is a 1 of 36^11 chance that this is true
            // But if that ever happens we might lose data, 
            // hence we still check to be sure
            }while(System.IO.File.Exists(path));
            System.IO.File.Move(tmpPath, path);
            return filename;
        }

        [Authorize]
        [HttpPut("{documentId}")]
        public async Task<IActionResult> UpdateDocument([FromRoute] int projectId, [FromRoute] int componentTypeId, [FromBody] DocumentInfo model, [FromBody] string userId, [FromBody] string comment)
        {
            if (await resourceAccess.GetRankForProject(authorizedUser, projectId) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }

            if (!await repository.UpdateDocument(model, userId, comment))
            {
                return new BadRequestObjectResult("Failed to update document: " + model.id);
            }

            return Ok();
        }

        [Authorize]
        [HttpPut("{documentId}")]
        public async Task<IActionResult> DeleteDocument([FromRoute] int projectId, [FromRoute] int documentId, [FromBody] string userId, [FromBody] string comment)
        {
            if (await resourceAccess.GetRankForProject(authorizedUser, projectId) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }

            if (!await repository.DeleteDocument(documentId, userId, comment))
            {
                return new BadRequestObjectResult("Failed to delete document: " + documentId);
            }

            return Ok();
        }


        [Authorize]
        [HttpGet("{documentId}/revisions")]
        public async Task<IActionResult> getRevisions([FromRoute]int documentId)
        {
            if (await resourceAccess.GetRankForProject(authorizedUser, projectId) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }

            RevisionInfo[] revisions = await repository.GetRevisionsAsync(documentId);
            return Ok(revisions);
        }
        
        [Authorize]
        [HttpPut("{documentId}")]
        public async Task<IActionResult> rollbackDocument([FromRoute] int projectId, [FromBody] RollbackProjectMarshallObject marshall)
        {
            if (await resourceAccess.GetRankForProject(authorizedUser, projectId) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }

            if (!await repository.RollbackDocument(marshall.revision, authorizedUser, marshall.comment))
            {
                return new BadRequestObjectResult("Failed to rollback log: " + marshall.revision);
            }

            return Ok();
        }
    }
}