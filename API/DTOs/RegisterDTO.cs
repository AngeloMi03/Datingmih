using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string username { get; set; }

        [Required]
        public string knowAs { get; set; }

        [Required]
        public string gender { get; set; }

        [Required]
        public DateTime dateOfBirth { get; set; }

        [Required]
        public string city { get; set; }

        [Required]
        public string country { get; set; }

        [Required]
        [StringLength(8, MinimumLength=4)]
        public string Password { get; set; }
    }
}