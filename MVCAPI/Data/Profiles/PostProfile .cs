using AutoMapper;
using MVCAPI.Data.Models;
using static MVCAPI.Data.DTOs.PostDTOs;

namespace MVCAPI.Data.Profiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            //source==>Targetr
            CreateMap<Post, PostReadDto>();
            CreateMap<PostNewOrUpdatedDto, Post>().ReverseMap();
        }
    }
}
