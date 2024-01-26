using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<AppUser, MemberDto>()
        .ForMember(
            dest => dest.PhotoUrl,
            opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.isMain).Url))
        .ForMember(
            dest => dest.Age,
            opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
        CreateMap<Photo, PhotoDto>();

        CreateMap<MemberUpdateDto, AppUser>();

        CreateMap<RegisterDto, AppUser>();

        // CreateMap<Message, MessageDto>()
        // .ForMember(dest => dest.SenderPhotoUrl,
        // opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(x => x.isMain).Url))
        // .ForMember(dest => dest.RecipientPhotoUrl,
        // opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(x => x.isMain).Url));

        CreateMap<Message, MessageDto>()
            .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.Photos
                .FirstOrDefault(x => x.isMain).Url))
            .ForMember(d => d.RecipientPhotoUrl, o => o.MapFrom(s => s.Recipient.Photos
                .FirstOrDefault(x => x.isMain).Url));

        // CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        // CreateMap<DateTime?, DateTime?>().ConstructUsing(d => d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);

        CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue ?
            DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
    }
}
