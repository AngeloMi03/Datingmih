using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using API.Helpers;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly Datacontext _datacontext;
        private readonly IMapper _mapper;
        public UserRepository(Datacontext datacontext, IMapper mapper)
        {
            _datacontext = datacontext;
            _mapper = mapper;

        }

        public async Task<membresDTO> GetMemberASync(string username)
        {
            
            var user = await _datacontext.Users
               .Where(x => x.UserName == username)
               .ProjectTo<membresDTO>(_mapper.ConfigurationProvider)
               .SingleOrDefaultAsync();

            return user!;
        }

        public async Task<PageList<membresDTO>> GetMembersASync(UseParams useParams)
        {
            var query = _datacontext.Users.AsQueryable();      

             query = query.Where(u => u.UserName != useParams.CurrentUsername);
             query = query.Where(u => u.Gender == useParams.Gender);

             var minDob = DateTime.Today.AddYears(-useParams.MaxAge - 1);
             var maxDob = DateTime.Today.AddYears(-useParams.MinAge);

             query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
             
             query = useParams.OrderBy switch{
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
             };

            return await PageList<membresDTO>.CreateAsync(query.ProjectTo<membresDTO>(_mapper.
             ConfigurationProvider).AsNoTracking()
              ,useParams.PageNumber, useParams.PageSize);
        }

        public async Task<AppUsers?> GetUserByUserNameAsync(string username) => await _datacontext.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);

        public async Task<IEnumerable<AppUsers?>> GetUsersAsync()
        {
            return await _datacontext.Users
            .Include(p => p.Photos)
            .ToListAsync();
        }

        public async Task<AppUsers?> GetUsersByIdAsync(int id)
        {
            return await _datacontext.Users.FindAsync(id);
        }

        public async Task<bool> SaveAllAsync()
        {
           return  await _datacontext.SaveChangesAsync() > 0;
        }
        public void Update(AppUsers user)
        {
           _datacontext.Entry(user).State = EntityState.Modified;
        }
    }
}