using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VCAPI.Options
{
    //The class gets filled out by the settings.JSON file. 
    public class JWTOptions
    {
        public string secretKey { get; set; }
        public string audience { get; set; }
    }
}
