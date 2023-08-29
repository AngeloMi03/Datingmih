using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUsers, membresDTO>()
            .ForMember(dest => dest.PhotoUrl, opt => 
              opt.MapFrom(src => src.Photos.FirstOrDefault( el => el.IsMain).url))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<Photo, PhotoDTO>();

            CreateMap<MemberUpdateDTO, AppUsers>();

            CreateMap<RegisterDTO, AppUsers>();

            CreateMap<Message, MessageDTO>()
             .ForMember(dest => dest.SenderPhotoUrl, opt => 
               opt.MapFrom( src => src.Sender.Photos.FirstOrDefault(x => x.IsMain).url))
             .ForMember(dest => dest.RecipientPhotoUrl, opt => 
               opt.MapFrom( src => src.Sender.Photos.FirstOrDefault(x => x.IsMain).url));
        }
    }
}