using AutoMapper;
using Dobrasync.Api.BusinessLogic.Dtos;
using Dobrasync.Api.BusinessLogic.Dtos.Versions;
using Dobrasync.Api.Database.Entities;
using Version = Dobrasync.Api.Database.Entities.Version;

namespace Dobrasync.Api.BusinessLogic.Mapper;

public class VersionMappingProfile : Profile
{
    public VersionMappingProfile()
    {
        CreateMap<Version, VersionDto>();
        CreateMap<VersionCreateDto, Version>();
    }
}