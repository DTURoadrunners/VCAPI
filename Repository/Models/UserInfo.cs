using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VCAPI.Repository.Models
{
    /// <summary>
    /// Contains all a users infomation, the password is stored in bytes.
    /// </summary>
    public class UserInfo
    {
        public string userID;
        public byte[] password;
        public string firstname;
        public string lastname;
        public int  phonenumber;
        public bool superuser;
    }
}
