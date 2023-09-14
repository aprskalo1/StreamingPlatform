using AutoMapper;
using RWAProject.Models;
using RWAProjectApi.DTOs;

namespace RWAProjectApi.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
             CreateMap<Video, VideoDTO>();
             CreateMap<VideoDTO, Video>().ForMember(d => d.CreatedAt, o => o.MapFrom((u, d) => d.CreatedAt = DateTime.UtcNow));
        }
    }
}
