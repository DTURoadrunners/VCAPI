using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface IDocumentRepository
    {
        Task<bool> CreateDocument(DocumentInfo info, LogInfo log, int ID);
        Task<bool> UpdateDocument(DocumentInfo info, LogInfo log);
        Task<bool> DeleteDocument(int ID, LogInfo log);
        Task<bool> RollbackDocument(int ID, LogInfo log);
        Task<DocumentInfo> GetDocument(int Id);
        Task<List<DocumentInfo>> getDocuments(int id);
    }
}
