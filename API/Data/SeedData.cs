using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using API.Entities;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    public class SeedData
    {
      //Datacontext datacontext params
        public static async Task seedData(UserManager<AppUsers> userManager , RoleManager<AppRole>  roleManager){
          if(await userManager.Users.AnyAsync()) return;

          var Userdata = await File.ReadAllTextAsync("Data/UserData.json");

          var users = JsonSerializer.Deserialize<List<AppUsers>>(Userdata);
          if(users == null) return;

          var roles = new List<AppRole>()
          {
              new AppRole {Name = "Admin"},
              new AppRole {Name = "Member"},
              new AppRole {Name = "Moderator"}
          };

          foreach(var role in roles)
          {
            await roleManager.CreateAsync(role);
          }

          foreach(var user in users!) {

            /*using var hmac = new HMACSHA512();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password"));
            user.PasswordSalt = hmac.Key;*/

            user.UserName = user.UserName.ToLower();
            //datacontext.Users.Add(user);

           await userManager.CreateAsync(user, "Password12");
           await userManager.AddToRolesAsync(user, new [] {"Member"});
          }

          //await datacontext.SaveChangesAsync();

          var admin = new AppUsers()
          {
            UserName = "admin"
          };

          await userManager.CreateAsync(admin, "Password12");
           await userManager.AddToRolesAsync(admin, new [] {"Admin", "Moderator"});
        }
    }
}