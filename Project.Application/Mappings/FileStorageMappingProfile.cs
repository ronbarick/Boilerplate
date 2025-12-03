using AutoMapper;
using Project.Domain.Dtos.FileStorage;
using Project.Domain.Entities;

namespace Project.Application.Mappings;

/// <summary>
/// AutoMapper profile for file storage DTOs
/// </summary>
public class FileStorageMappingProfile : Profile
{
    public FileStorageMappingProfile()
    {
        CreateMap<FileStorageItem, FileInfoDto>()
            .ForMember(dest => dest.PublicUrl, opt => opt.Ignore()); // Populated separately if needed
    }
}
