using AutoMapper;
using ClinAgend.Models.DTOs;
using ClinAgend.Models.Models;
using ClinAgend.Web.Helpers;

namespace ClinAgend.Web.Profiles
{
    internal class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            // Entidade → DTO
            CreateMap<Appointment, AppointmentDTO>()
                .ForMember(dest => dest.StartDate,
                    opt => opt.MapFrom(src => DateTimeConverter.ToDateString(src.StartTime)))
                .ForMember(dest => dest.EndDate,
                    opt => opt.MapFrom(src => DateTimeConverter.ToDateString(src.EndTime)))
                .ForMember(dest => dest.StartTime,
                    opt => opt.MapFrom(src => DateTimeConverter.ToTimeString(src.StartTime)))
                .ForMember(dest => dest.EndTime,
                    opt => opt.MapFrom(src => DateTimeConverter.ToTimeString(src.EndTime)))
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : string.Empty));

            // DTO → Entidade
            CreateMap<AppointmentDTO, Appointment>()
                .ForMember(dest => dest.StartTime,
                    opt => opt.MapFrom(src => DateTimeConverter.ParseDateTime(src.StartDate, src.StartTime)))
                .ForMember(dest => dest.EndTime,
                    opt => opt.MapFrom(src => DateTimeConverter.ParseDateTime(src.EndDate, src.EndTime)))
                .ForMember(dest => dest.Patient, opt => opt.Ignore())
                .ForMember(dest => dest.Doctor, opt => opt.Ignore())
                .ForMember(dest => dest.Clinic, opt => opt.Ignore());
        }
    }
}
