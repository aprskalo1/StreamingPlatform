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
            CreateMap<Notification, NotificationDTO>();
            CreateMap<NotificationDTO, Notification>()
                .ForMember(d => d.CreatedAt, o => o.MapFrom((u, d) => d.CreatedAt = DateTime.UtcNow))
                .ForMember(d => d.SentAt, o => o.MapFrom(u => DateTime.UtcNow));
        }
    }
}
