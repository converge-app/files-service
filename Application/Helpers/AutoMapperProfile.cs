using Application.Models.DataTransferObjects;
using Application.Models.Entities;
using AutoMapper;

namespace Application.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<File, FileDto>();
            CreateMap<FileDto, File>();
            CreateMap<FileUpdateDto, File>();
            CreateMap<FileCreationDto, File>();
        }
    }
}