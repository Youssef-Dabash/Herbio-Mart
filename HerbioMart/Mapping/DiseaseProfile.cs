using AutoMapper;
using HerbioMart.Models.Entities;
using HerbioMart.ViewModels.Diseases;

namespace HerbioMart.Mapping;

public class DiseaseProfile : Profile
{
    public DiseaseProfile()
    {
        // Map from database entity to client ViewModel
        CreateMap<Disease, DiseaseVM>();


    }
}