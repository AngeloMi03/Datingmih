using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class ClaimPrincipalExtension
    {

        public static string GetUserName(this ClaimsPrincipal User){
            return  User.FindFirst(ClaimTypes.Name)?.Value;

        }
             
       public static int GetUserId(this ClaimsPrincipal User) {
           return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
       }
            
    }
}