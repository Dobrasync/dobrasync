using AutoMapper;
using Dobrasync.Api.BusinessLogic.Dtos;
using File = Dobrasync.Api.Database.Entities.File;

namespace Dobrasync.Api.BusinessLogic.Mapper;

public class FileMappingProfile : Profile
{
    public FileMappingProfile()
    {
        CreateMap<File, FileDto>();
    }
}