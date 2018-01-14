using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VCAPI.Filters;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.Models;
using VCAPI.Repository;

namespace VCAPI.Controllers
{
    [Route("api/project/{projectId}/componentType/{componentTypeId}/[controller]")]
    public class DocumentController : Controller
    {
        private readonly IDocumentRepository repository;
        private readonly IResourceAccess resourceAccess;

        public DocumentController(IDocumentRepository repository, IResourceAccess resourceAccess)
        {
            this.repository = repository;
            this.resourceAccess = resourceAccess;
        }


        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetDocument([FromRoute]int documentId)
        {
            DocumentInfo document = await repository.GetDocument(documentId);
            if (document != null)
            {
                return Ok(document);
            }
            else
            {
                return BadRequest("No such document");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDocuments([FromRoute]int componentTypeId)
        {
            List<DocumentInfo> documents = await repository.getDocuments(componentTypeId);
            if (documents != null)
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
        public async Task<IActionResult> CreateDocument([FromRoute] int projectId, [FromRoute] int componentTypeId, [FromBody] DocumentInfo model, [FromBody] string userId, [FromBody] string comment)
        {
            if (await resourceAccess.GetRankForProject(User.Identity.Name, projectId) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }

            int id = await repository.CreateDocument(model, userId, comment, componentTypeId);
            if (id == -1)
            {
                return new BadRequestObjectResult("Failed to create component");
            }
            return Created("api/project/" + projectId + "/componentType/" + componentTypeId + "/document/" + id, null);
        }

        [Authorize]
        [HttpPut("{documentId}")]
        public async Task<IActionResult> UpdateDocument([FromRoute] int projectId, [FromRoute] int componentTypeId, [FromBody] DocumentInfo model, [FromBody] string userId, [FromBody] string comment)
        {
            if (await resourceAccess.GetRankForProject(User.Identity.Name, projectId) < Repository.RANK.STUDENT)
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
            if (await resourceAccess.GetRankForProject(User.Identity.Name, projectId) < Repository.RANK.STUDENT)
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
        [HttpPut("{documentId}")]
        public async Task<IActionResult> rollbackDocument([FromRoute] int projectId, [FromRoute] int logId, [FromBody] string userId, [FromBody] string comment)
        {
            if (await resourceAccess.GetRankForProject(User.Identity.Name, projectId) < Repository.RANK.STUDENT)
            {
                return Unauthorized();
            }

            if (!await repository.RollbackDocument(logId, userId, comment))
            {
                return new BadRequestObjectResult("Failed to rollback log: " + logId);
            }

            return Ok();
        }
    }
}