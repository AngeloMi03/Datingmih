using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class DTOUSER
    {
        public String? UserName { get; set; }
        public String? Token { get; set; }
        public String? PhotoUrl { get; set; }
        public String? KnowAs { get; set; }
        public String? Gender { get; set; }
    }
}