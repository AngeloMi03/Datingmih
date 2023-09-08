using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using API.DTOs;
using API.Interface;
using API.Service;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace API.Controllers
{
    public class AccounteController : BaseApiController
    {
        private readonly SignInManager<AppUsers> _signInManager;
        public readonly ItokenService _TokenService;
        private readonly IMapper _mapper;
        public UserManager<AppUsers> _userManager { get; }

        //public readonly Datacontext _dbcontext;
        //Datacontext dbcontext, params
        public AccounteController(UserManager<AppUsers> userManager,SignInManager<AppUsers> signInManager, ItokenService TKService, IMapper mapper)
        {
            _userManager = userManager;
           _signInManager = signInManager;
            _TokenService = TKService;
            _mapper = mapper;
              //_dbcontext = dbcontext;
        }

        [HttpPost("register")]
        public async Task<ActionResult<DTOUSER>> Register(RegisterDTO UserData)
        {
            if(await UsernameExist(UserData.username)){
                return BadRequest("username already exist!!");
            }


           /* using var hash = new HMACSHA512();
            user.PasswordHash = hash.ComputeHash(Encoding.UTF8.GetBytes(UserData.Password));
            user.PasswordSalt = hash.Key;*/

            var user = _mapper.Map<AppUsers>(UserData);
    
            user.UserName = UserData.username.ToLower();
            
            var result = await _userManager.CreateAsync(user, UserData.Password);

            if(!result.Succeeded) return BadRequest(result.Errors);
            //_dbcontext.Users?.Add(user);
            //await _dbcontext.SaveChangesAsync();

            var role = await _userManager.AddToRoleAsync(user, "Member");

            if(!role.Succeeded) return BadRequest(role.Errors);

            return new DTOUSER{
                Username = user.UserName,
                Token = await _TokenService.CreationToken(user),
                KnowAs = user.KnowAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<DTOUSER?>> Login(LoginDto UserData){
            var user = await _userManager.Users //await _dbcontext.Users
            .Include(x => x.Photos)
            .SingleOrDefaultAsync(u => u.UserName == UserData.username);

            if(user == null){
               return Unauthorized("Invalid User !!!!");
            }

            /*using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedhas = hmac.ComputeHash(Encoding.UTF8.GetBytes(UserData.Password));

            for(var i =0; i < computedhas.Length; i++){
                if(computedhas[i] != user.PasswordHash[i]){
                    return Unauthorized("Password incorrect");
                }
            }*/

            var result = await _signInManager.CheckPasswordSignInAsync(user, UserData.Password, false);

            if(!result.Succeeded) return Unauthorized();

            return new DTOUSER{
                Username = user.UserName,
                Token = await _TokenService.CreationToken(user),
                PhotoUrl = user?.Photos?.FirstOrDefault(x => x.IsMain)?.url,
                KnowAs = user?.KnowAs,
                Gender = user?.Gender
            };
        }

        private async Task<bool> UsernameExist(string username){ 
            return await _userManager.Users.AnyAsync(u => u.UserName == username);
        }

    }
}