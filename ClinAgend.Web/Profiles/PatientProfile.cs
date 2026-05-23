using AutoMapper;
using ClinAgend.Models.DTOs;
using ClinAgend.Models.Models;
using System;
using System.Globalization;

namespace ClinAgend.Web.Profiles
{
    internal class PatientProfile : Profile
    {
        public PatientProfile()
        {
            // DTO -> Model
            CreateMap<PatientDTO, Patient>()
                .ForMember(dest => dest.BirthDate,
                           opt => opt.MapFrom(src => DateTime.ParseExact(
                               src.BirthDate,
                               "dd/MM/yyyy",
                               CultureInfo.InvariantCulture)));

            // Model -> DTO
            CreateMap<Patient, PatientDTO>()
                .ForMember(dest => dest.BirthDate,
                           opt => opt.MapFrom(src => src.BirthDate.ToString("dd/MM/yyyy")));

            // Model -> Lookup DTO
            CreateMap<Patient, PatientLookupDTO>();
        }
    }
}
