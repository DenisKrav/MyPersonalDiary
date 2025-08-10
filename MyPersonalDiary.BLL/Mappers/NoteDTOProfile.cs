using AutoDependencyRegistration.Attributes;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using MyPersonalDiary.BLL.DTOs.Note.Response;
using MyPersonalDiary.BLL.DTOs.User.Request;
using MyPersonalDiary.BLL.DTOs.User.Response;
using MyPersonalDiary.DAL.Models;
using MyPersonalDiary.DAL.Models.Identities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.Mappers
{
    public class DecryptDiaryRecordContentResolver : IValueResolver<DiaryRecord, NoteResponseDto, string?>
    {
        private readonly IDataProtector _protector;
        private readonly IConfiguration _config;

        public DecryptDiaryRecordContentResolver(IDataProtectionProvider provider, IConfiguration config)
        {
            _config = config;
            _protector = provider.CreateProtector(_config["Protector:ContentProtector"]);
        }

        public string? Resolve(DiaryRecord source, NoteResponseDto destination, string? destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.EncryptedContent))
                return null;

            try
            {
                return _protector.Unprotect(source.EncryptedContent);
            }
            catch
            {
                return null;
            }
        }
    }

    public class DecryptDiaryImageDataResolver : IValueResolver<DiaryImage, NoteResponseDto, byte[]?>
    {
        private readonly IDataProtector _protector;
        private readonly IConfiguration _config;

        public DecryptDiaryImageDataResolver(IDataProtectionProvider provider, IConfiguration config)
        {
            _config = config;
            _protector = provider.CreateProtector(_config["Protector:ContentProtector"]);
        }

        public byte[]? Resolve(DiaryImage source, NoteResponseDto destination, byte[]? destMember, ResolutionContext context)
        {
            if (source.Data is null || source.Data.Length == 0) return null;
            try
            {
                return _protector.Unprotect(source.Data);
            }
            catch
            {
                return null;
            }
        }
    }

    public class NoteDTOProfile : Profile
    {
        public NoteDTOProfile()
        {
            CreateMap<DiaryRecord, NoteResponseDto>()
                .ForMember(d => d.Content, opt => opt.MapFrom<DecryptDiaryRecordContentResolver>())
                .ForMember(d => d.ImageContentType, opt => opt.Ignore())
                .ForMember(d => d.ImageSize, opt => opt.Ignore())
                .ForMember(d => d.ImageData, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => s.CreatedAt));

            CreateMap<DiaryImage, NoteResponseDto>()
                .ForMember(d => d.Content, opt => opt.Ignore())
                .ForMember(d => d.ImageContentType, opt => opt.MapFrom(s => s.ContentType))
                .ForMember(d => d.ImageSize, opt => opt.MapFrom(s => (long?)s.Size))
                .ForMember(d => d.ImageData, opt => opt.MapFrom<DecryptDiaryImageDataResolver>())
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => s.UploadedAt));
        }
    }
}
