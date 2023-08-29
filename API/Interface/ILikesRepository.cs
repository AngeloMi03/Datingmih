using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interface
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        Task<AppUsers> GetUserWithLike(int userId);
        Task<PageList<LikeDTO>> GetUserLikes(LikesParams likesParams);
    }
}