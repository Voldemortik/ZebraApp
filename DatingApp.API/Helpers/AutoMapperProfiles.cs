using System;
using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Model;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles ()
        {
            CreateMap<User,UserToListDto>()
            .ForMember(dest=>dest.PhotoUrl,opt=>{
                opt.MapFrom(src=>src.Photos.FirstOrDefault(p=>p.IsMain).Url);
            })
             .ForMember(dest=>dest.Age,opt=>{
                opt.ResolveUsing(src=>src.DateOfBirth.Calculate());
            });
            
            CreateMap<User,UserForDetailedDto>()
            .ForMember(dest=>dest.PhotoUrl,opt=>{
                opt.MapFrom(src=>src.Photos.FirstOrDefault(p=>p.IsMain).Url);
            })
            .ForMember(dest=>dest.Age,opt=>{
                opt.ResolveUsing(src=>src.DateOfBirth.Calculate());
            });
            CreateMap<Photo,PhotosForDetailedDto>();
            CreateMap<UserForUpdateDto,User>();
        }
    }
}