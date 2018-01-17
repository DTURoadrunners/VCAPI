using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface IDocumentRepository
    {
        /// <summary>
        /// Creates a document
        /// </summary>
        /// <param name="info">The data need to create a document</param>
        /// <param name="userId">Checks if the user is eligible</param>
        /// <param name="comment">Used in the logs</param>
        /// <param name="id">The documents unique id</param>
        /// <returns></returns>
        Task<int> CreateDocument(DocumentInfo info, string userId, string comment, int id);
        Task<bool> UpdateDocument(DocumentInfo info, string userId, string comment);
        Task<bool> DeleteDocument(int id, string userId, string comment);
        Task<bool> RollbackDocument(int id, string userId, string comment);
        Task<RevisionInfo[]> GetRevisionsAsync(int documentId);
        Task<DocumentInfo> GetDocument(int id);
        Task<List<DocumentInfo>> getDocuments(int id);
    }
}
