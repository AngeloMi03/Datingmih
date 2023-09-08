using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interface;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWorks : IUnitOfWorks
    {
        private readonly Datacontext _Context;
        private readonly IMapper _Mapper;
        public UnitOfWorks(Datacontext context, IMapper mapper)
        {
            _Mapper = mapper;
            _Context = context;
        }

        public IUserRepository? UserRepository => new UserRepository(_Context, _Mapper);

        public IMessageRepository? MessageRepository => new MessageRepository(_Context, _Mapper);

        public ILikesRepository? LikesRepository => new LikesRepositry(_Context);

        public async Task<bool> Complete()
        {
            return await _Context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _Context.ChangeTracker.HasChanges();
        }
    }
}