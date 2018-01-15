using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface IDocumentRepository
    {
        Task<int> CreateDocument(DocumentInfo info, string userId, string comment, int id);
        Task<bool> UpdateDocument(DocumentInfo info, string userId, string comment);
        Task<bool> DeleteDocument(int id, string userId, string comment);
        Task<bool> RollbackDocument(int id, string userId, string comment);
        Task<RevisionInfo[]> GetRevisionAsync(int documentId);
        Task<DocumentInfo> GetDocument(int id);
        Task<List<DocumentInfo>> getDocuments(int id);
    }
}
