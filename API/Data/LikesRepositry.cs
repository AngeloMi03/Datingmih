using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interface;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepositry : ILikesRepository
    {
        private readonly Datacontext _datacontext;
        public LikesRepositry(Datacontext datacontext)
        {
            _datacontext = datacontext;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _datacontext.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<PageList<LikeDTO>> GetUserLikes(LikesParams likesParams)
        {
            var users = _datacontext.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _datacontext.Likes!.AsQueryable();

            if(likesParams.Predicate == "liked"){
                likes = likes.Where(x => x.SourceUserID == likesParams.UserId);
                users = likes.Select(x => x.LikedUser!);
            }

            
            if(likesParams.Predicate == "likedBy"){
                likes = likes.Where(x => x.LikedUserID == likesParams.UserId);
                users = likes.Select(x => x.SourceUser!);
            }

           var likedUsers = users!.Select(user => new LikeDTO{
              id = user.Id,
              username = user.UserName,
              age = user.DateOfBirth.CalculateAge(),
              KnowAs = user.KnowAs,
              PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).url,
              city = user.City
            });

            return await PageList<LikeDTO>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUsers> GetUserWithLike(int userId)
        {
            return await _datacontext.Users
              .Include( s => s.LikedUsers)
              .FirstOrDefaultAsync(s => s.Id  == userId);
              
              
        }
    }
}