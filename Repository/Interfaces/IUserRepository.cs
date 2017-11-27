﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCAPI.Repository.Models;

namespace VCAPI.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> CreateUser(UserInfo info);
        Task<bool> Authenticate(string username, string password);
        Task<UserInfo> GetUser(string username);        
    }
}
