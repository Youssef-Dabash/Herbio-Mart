using AutoMapper;
using HerbioMart.Models.Entities;
using HerbioMart.ViewModels.Diseases;

namespace HerbioMart.Mapping;

public class DiseaseProfile : Profile
{
    public DiseaseProfile()
    {
        CreateMap<Disease, DiseaseVM>();
        CreateMap<CreateDiseaseVM, Disease>()
                .ForMember(dest => dest.DiseaseName, opt => opt.MapFrom(src => src.DiseaseName.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.Trim()))
                .ForMember(dest => dest.Symptoms, opt => opt.MapFrom(src => src.Symptoms.Trim()))
                .ForMember(dest => dest.DiseaseId, opt => opt.Ignore());

    }
}