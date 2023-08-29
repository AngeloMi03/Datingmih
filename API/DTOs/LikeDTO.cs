using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class LikeDTO
    {
        public int id { get; set; }
        public string? username { get; set; }
        public int age { get; set; }
        public string? KnowAs { get; set; }
        public string? PhotoUrl { get; set; }
        public string? city { get; set; }

    }
}