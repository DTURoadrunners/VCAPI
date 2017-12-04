using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface IDocumentRepository
    {
        Task<int> CreateDocument(DocumentInfo info, string userId, string comment, int ID);
        Task<bool> UpdateDocument(DocumentInfo info, string userId, string comment, int ID);
        Task<bool> DeleteDocument(int ID, string userId, string comment);
        Task<bool> RollbackDocument(int ID, string userId, string comment);
        Task<DocumentInfo> GetDocument(int Id);
        Task<List<DocumentInfo>> getDocuments(int id);
    }
}
