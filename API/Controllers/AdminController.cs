using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUsers> _userManager;
        public AdminController(UserManager<AppUsers> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpGet("users-with-role")]
        public async Task<ActionResult> GetUserWithRole()
        {
            var users = await _userManager.Users!
             .Include(x => x.UserRoles!)
             .ThenInclude(x => x.Role)
             .OrderBy(x => x.UserName)
             .Select(x => new {
                x.Id,
               Username =  x.UserName,
               Role = x.UserRoles!.Select(u => u.Role!.Name).ToList()
             })
             .ToListAsync();

            return Ok(users);
        }

         [Authorize(Policy = "RequiredAdminRole")]
         [HttpPost("edit-role/{username}")]
         public async Task<ActionResult> EditRole(string username, [FromQuery] string roles )
         {
            var selectedRole = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);
            if(user is null) return NotFound("User not found");

            var userRole = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRole.Except(userRole));

            if(!result.Succeeded) return BadRequest("failed to add Role");

            result = await _userManager.RemoveFromRolesAsync(user, userRole.Except(selectedRole));

            if(!result.Succeeded) return  BadRequest("failed to remove Role");

            return Ok(await _userManager.GetRolesAsync(user));

         }


        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photo-to-moderate")]
        public ActionResult GetPhotoForModeration()
        {
            return Ok("only moderator or admin can see this");
        }
    }
}