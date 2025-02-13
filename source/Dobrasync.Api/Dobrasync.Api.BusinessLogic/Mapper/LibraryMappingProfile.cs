using AutoMapper;
using Dobrasync.Api.BusinessLogic.Dtos;
using Dobrasync.Api.Database.Entities;
using File = Dobrasync.Api.Database.Entities.File;

namespace Dobrasync.Api.BusinessLogic.Mapper;

public class LibraryMappingProfile : Profile
{
    public LibraryMappingProfile()
    {
        CreateMap<Library, LibraryDto>();
    }
}