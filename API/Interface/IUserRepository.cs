using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interface
{
    public interface IUserRepository
    {
        void Update(AppUsers user);

        //Task<bool> SaveAllAsync();

        Task<IEnumerable<AppUsers?>> GetUsersAsync();

        Task<AppUsers?> GetUsersByIdAsync(int id);

        Task<AppUsers?> GetUserByUserNameAsync(string username);

        Task<PageList<membresDTO>> GetMembersASync(UseParams useParams);
        Task<membresDTO> GetMemberASync(string username);


    }
}