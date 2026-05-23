using AutoMapper;
using ClinAgend.Models.DTOs;
using ClinAgend.Models.Models;

namespace ClinAgend.Web.Profiles
{
    internal class ClinicProfile : Profile
    {
        public ClinicProfile()
        {
            CreateMap<Clinic, ClinicDTO>();

            CreateMap<ClinicDTO, Clinic>()
                .ForMember(dest => dest.Settings,
                    opt => opt.Ignore());
        }
    }
}
