using AutoMapper;
using ClinAgend.Models.DTOs;
using ClinAgend.Models.Models;
using ClinAgend.Web.Helpers;

namespace ClinAgend.Web.Profiles
{
    internal class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            // Entidade → DTO
            CreateMap<Doctor, DoctorDTO>()
                .ForMember(dest => dest.StartTime,
                    opt => opt.MapFrom(src => TimeSpanConverter.ToStringFormat(src.StartTime)))
                .ForMember(dest => dest.EndTime,
                    opt => opt.MapFrom(src => TimeSpanConverter.ToStringFormat(src.EndTime)));

            // DTO → Entidade
            CreateMap<DoctorDTO, Doctor>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.StartTime,
                    opt => opt.MapFrom(src => TimeSpanConverter.ParseOrDefault(src.StartTime)))
                .ForMember(dest => dest.EndTime,
                    opt => opt.MapFrom(src => TimeSpanConverter.ParseOrDefault(src.EndTime)));
        }
    }
}
