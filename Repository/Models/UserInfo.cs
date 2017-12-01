﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VCAPI.Repository.Models
{
    public class UserInfo
    {
        public string userID;
        public byte[] password;
        public string firstname;
        public string lastname;

        public UserInfo(string userID, string firstname, string lastname)
        {
            this.userID = userID;
            this.firstname = firstname;
            this.lastname = lastname;
        }
    }
}
