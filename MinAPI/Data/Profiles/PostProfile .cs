using AutoMapper;
using MinAPI.Data.Models;
using static MinAPI.Data.DTOs.PostDTOs;

namespace MinAPI.Data.Profiles
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
