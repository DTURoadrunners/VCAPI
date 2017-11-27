using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface IDocumentRepository
    {
        Task<bool> CreateDocument(DocumentInfo info, string userid, string comment, int ID);
        Task<bool> UpdateDocument(DocumentInfo info, string userid, string comment, int activeID);
        Task<bool> DeleteDocument(int ID, string userid, string comment);
        Task<bool> RollbackDocument(int ID, string userid, string comment);
        Task<DocumentInfo> GetDocument(int Id);
        Task<List<DocumentInfo>> getDocuments(int id);
    }
}
