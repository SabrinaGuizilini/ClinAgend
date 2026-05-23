using AutoMapper;
using ClinAgend.Models.DTOs;
using ClinAgend.Models.Models;

namespace ClinAgend.Web.Profiles;

internal class ClinicSettingsProfile : Profile
{
    public ClinicSettingsProfile()
    {
        CreateMap<ClinicSettings, ClinicSettingsDTO>();

        CreateMap<ClinicSettingsDTO, ClinicSettings>()
            .ForMember(dest => dest.Clinic,
                opt => opt.Ignore());
    }
}