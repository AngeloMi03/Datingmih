using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;
        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _likesRepository = likesRepository;
            _userRepository = userRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
           var sourceUserId  = User.GetUserId();
           var likedUser = await _userRepository.GetUserByUserNameAsync(username);
           var sourceUser = await _likesRepository.GetUserWithLike(sourceUserId);

           if(likedUser == null) return NotFound("User not found");

           if(sourceUser.UserName == username) return BadRequest("you can not like yourself");

           var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

           if(userLike != null ) return BadRequest("you have already liked this user");

           userLike =  new UserLike
            {
               SourceUserID = sourceUserId,
               LikedUserID = likedUser.Id
            };

            sourceUser?.LikedUsers?.Add(userLike);

            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetUsersLike([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var user = await _likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(user.CurrentPage,user.PageSize,user.TotalCount,user.TotalPage);

            return Ok(user);
        }
    }
}