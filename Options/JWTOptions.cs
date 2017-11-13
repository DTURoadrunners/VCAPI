using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VCAPI.Options
{
    public class JWTOptions
    {
        public string secretKey { get; set; }
        public string audience { get; set; }
    }
}
