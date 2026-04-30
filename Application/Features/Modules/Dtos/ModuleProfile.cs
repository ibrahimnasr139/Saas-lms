using Application.Features.Modules.Commands.CreateModule;
using Application.Features.Modules.Commands.UpdateModule;

namespace Application.Features.Modules.Dtos
{
    public sealed class ModuleProfile : Profile
    {
        public ModuleProfile()
        {
            CreateMap<CreateModuleCommand, Module>();

            CreateMap<UpdateModuleCommand, Module>();

            CreateMap<Module, AllModulesDto>()
                .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.ModuleItems.Count))
                .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src => src.Lessons.Count))
                .ForMember(dest => dest.Assignments, opt => opt.MapFrom(src => src.Assignments.Count))
                .ForMember(dest => dest.Quizzes, opt => opt.MapFrom(src => src.Quizzes.Count));
        }
    }
}
