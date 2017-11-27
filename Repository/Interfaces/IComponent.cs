using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface IComponent
    {
        Task<bool> CreateComponent(int activeComponentTypeID, Component component, LogInfo log);
        Task<Component> ReadComponent(string id);
        Task<bool> UpdateComponent(int activeComponentTypeID, Component component, LogInfo log);
        Task<bool> DeleteComponent(string activeID, LogInfo log);
    }
}
