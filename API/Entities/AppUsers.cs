using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class AppUsers : IdentityUser<int>
    {
       /* public int Id { get; set; }
        public string UserName { get; set; }

        public byte[] PasswordHash { get; set; } 
        public byte[] PasswordSalt { get; set; }*/
        public DateTime DateOfBirth { get; set; }
        public string? KnowAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string? Gender { get; set; }
        public string? Introduction { get; set; }
        public string? Lookingfor { get; set; }
        public string? Interests  { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        public ICollection<Photo>? Photos { get; set; }

        public ICollection<UserLike>? LikedByUsers { get; set; }
        public ICollection<UserLike>? LikedUsers { get; set; }
        public ICollection<Message>? MessageSent { get; set; }
        public ICollection<Message>? MessageReceveid { get; set; }

       public ICollection<AppUserRole>? UserRoles {get; set;}

        /*public int GetAge(){
            return DateOfBirth.CalculateAge();
        }*/




    }
}