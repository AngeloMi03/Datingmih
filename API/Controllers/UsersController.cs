using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Extensions;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        //public readonly IUserRepository _unitOfWorks.UserRepository!;
        public readonly IMapper _mapper;
        public readonly IPhotoService _photoService;
        private readonly IUnitOfWorks _unitOfWorks;
        public UsersController(IUnitOfWorks unitOfWorks, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWorks = unitOfWorks;
           // _unitOfWorks.UserRepository! = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }


  
        [HttpGet]
        public async Task<ActionResult<IEnumerable<membresDTO>>> GetUsers([FromQuery]UseParams useParams)
        {
            var username = User.GetUserName();
            var user = await _unitOfWorks.UserRepository!.GetUserByUserNameAsync(username!);

           useParams.CurrentUsername = user!.UserName;

           if(string.IsNullOrEmpty(useParams.Gender)){
             useParams.Gender =  user.Gender == "male" ? "female" : "male";
           }

            var Users = await _unitOfWorks.UserRepository!.GetMembersASync(useParams);
            
            Response.AddPaginationHeader(Users.CurrentPage, Users.PageSize, Users.TotalCount, Users.TotalPage);
            return Ok(Users);

        }


        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<membresDTO?>> GetUser(string username)
        {

            var user = await _unitOfWorks.UserRepository!.GetMemberASync(username);
            return user;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
        {

            var username = User.GetUserName();
            var user = await _unitOfWorks.UserRepository!.GetUserByUserNameAsync(username!);

            _mapper.Map(memberUpdateDTO, user);

            _unitOfWorks.UserRepository!.Update(user!);

            if (await _unitOfWorks.Complete()) return NoContent();

            return BadRequest("failed to save change");

        }


        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var username = User.GetUserName();
            var user = await _unitOfWorks.UserRepository!.GetUserByUserNameAsync(username!);

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            user?.Photos?.Add(photo);

            if (user?.Photos?.Count == 0)
            {
                photo.IsMain = true;
            }

            if (await _unitOfWorks.Complete())
            {
                return CreatedAtRoute("GetUser", new { username = user!.UserName }, _mapper.Map<PhotoDTO>(photo));

            }

            return BadRequest("Probleme Adding photo");

        }


        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var username = User.GetUserName();
            var user = await _unitOfWorks.UserRepository!.GetUserByUserNameAsync(username!);

            var photo = user?.Photos?.FirstOrDefault(x => x.id == photoId);
            if (photo!.IsMain) return BadRequest("this is already your main photo");

            var currentMain = user?.Photos?.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _unitOfWorks.Complete()) return NoContent();

            return BadRequest("Failed to set main photo");

        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var username = User.GetUserName();
            var user = await _unitOfWorks.UserRepository!.GetUserByUserNameAsync(username!);

            var photo = user?.Photos?.FirstOrDefault(x => x.id == photoId);
            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("you can not delete main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user?.Photos?.Remove(photo);

            if (await _unitOfWorks.Complete()) return Ok();

            return BadRequest("failed to delete photo");

        }

    }

}