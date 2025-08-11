using AutoDependencyRegistration.Attributes;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MyPersonalDiary.BLL.DTOs.Note.Request;
using MyPersonalDiary.BLL.DTOs.Note.Response;
using MyPersonalDiary.BLL.Exceptions;
using MyPersonalDiary.BLL.InterfacesServices;
using MyPersonalDiary.DAL.Enums;
using MyPersonalDiary.DAL.InterfacesRepositories;
using MyPersonalDiary.DAL.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.Services
{
    [RegisterClassAsTransient]
    public class NoteService : INoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDataProtector _protector;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;

        private readonly string _userNotesKeyPattern;
        private readonly TimeSpan _absoluteTtl;
        private readonly TimeSpan _slidingTtl;

        private const int TenMb = 10 * 1024 * 1024;

        public NoteService(IUnitOfWork unitOfWork, IMapper mapper, IDataProtectionProvider dataProtectionProvider, IConfiguration config, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = config;
            _protector = dataProtectionProvider.CreateProtector(_config["Protector:ContentProtector"]);
            _cache = cache;

            _userNotesKeyPattern = config["Cache:Keys:UserNotes"] ?? "notes:{userId}";
            var absMin = config.GetValue<int?>("Cache:TTL:AbsoluteMinutes") ?? 5;
            var slMin = config.GetValue<int?>("Cache:TTL:SlidingMinutes") ?? 2;

            _absoluteTtl = TimeSpan.FromMinutes(absMin);
            _slidingTtl = TimeSpan.FromMinutes(slMin);
        }

        public async Task<IEnumerable<NoteResponseDto>> GetUserNotesAsync(long userId)
        {
            var cacheKey = CacheKey(userId);

            if (_cache.TryGetValue(cacheKey, out IEnumerable<NoteResponseDto> cached))
                return cached;

            var diaryRecords = await _unitOfWork.DiaryRecordRepository.GetAsync(n => n.UserId == userId);
            var diaryImages = await _unitOfWork.DiaryImageRepository.GetAsync(n => n.UserId == userId);

            var notes = diaryRecords
                .Select(r => _mapper.Map<NoteResponseDto>(r))
                .Concat(diaryImages.Select(i => _mapper.Map<NoteResponseDto>(i)))
                .OrderByDescending(n => n.CreatedAt)
                .ToArray();

            _cache.Set(
                cacheKey,
                notes,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _absoluteTtl,
                    SlidingExpiration = _slidingTtl
                }
            );

            return notes;
        }

        public async Task<NoteResponseDto> AddNoteAsync(AddNoteRequestDto addNote)
        {
            if (addNote == null)
                throw new ArgumentNullException(nameof(addNote));

            if (!string.IsNullOrWhiteSpace(addNote.Content))
            {
                if (addNote.Content.Length > 500)
                    throw new ArgumentException("Note content exceeds 500 characters limit.");

                var diaryRecord = new DiaryRecord
                {
                    Id = Guid.NewGuid(),
                    UserId = addNote.UserId,
                    EncryptedContent = _protector.Protect(addNote.Content),
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.DiaryRecordRepository.InsertAsync(diaryRecord);
                await _unitOfWork.SaveAsync();

                InvalidateUserNotesCache(addNote.UserId);

                return _mapper.Map<NoteResponseDto>(diaryRecord);
            }
            else if (addNote.ImageData != null && addNote.ImageData.Length > 0)
            {
                byte[] finalImageData = addNote.ImageData;
                string finalContentType = addNote.ImageContentType ?? "application/octet-stream";

                if (finalImageData.Length > TenMb)
                {
                    (finalImageData, finalContentType) = await OptimizeImageAsync(finalImageData);
                }

                if (finalImageData.Length > TenMb)
                {
                    (finalImageData, finalContentType) = await OptimizeImageAsync(finalImageData, quality: 65);
                }

                var encryptedImage = _protector.Protect(finalImageData);

                var diaryImage = new DiaryImage
                {
                    Id = Guid.NewGuid(),
                    UserId = addNote.UserId,
                    FileName = $"note_{Guid.NewGuid()}",
                    ContentType = finalContentType,
                    Size = finalImageData.Length,
                    Data = encryptedImage,
                    UploadedAt = DateTime.UtcNow
                };

                await _unitOfWork.DiaryImageRepository.InsertAsync(diaryImage);
                await _unitOfWork.SaveAsync();

                InvalidateUserNotesCache(addNote.UserId);

                return _mapper.Map<NoteResponseDto>(diaryImage);
            }
            else
            {
                throw new ArgumentException("Note must contain either text or an image.");
            }
        }

        public async Task<Guid> DeleteNoteAsync(DeleteNoteRequestDto deleteRequest)
        {
            if (deleteRequest == null)
                throw new ArgumentNullException(nameof(deleteRequest));

            if (deleteRequest.NoteId == Guid.Empty)
                throw new ArgumentException("Note ID cannot be empty.", nameof(deleteRequest.NoteId));

            if (string.IsNullOrWhiteSpace(deleteRequest.NoteType))
                throw new ArgumentException("Note type cannot be empty.", nameof(deleteRequest.NoteType));

            if (!Enum.TryParse<NoteType>(deleteRequest.NoteType, ignoreCase: true, out var noteType))
                throw new ArgumentException("Invalid note type.", nameof(deleteRequest.NoteType));

            var cutoff = DateTime.UtcNow.AddDays(-2);
            long userIdToInvalidate;

            switch (noteType)
            {
                case NoteType.Text:
                {
                    var note = await _unitOfWork.DiaryRecordRepository.GetByIDAsync(deleteRequest.NoteId);
                    if (note is null)
                        throw new ArgumentException("Note not found.", nameof(deleteRequest.NoteId));

                    if (note.CreatedAt < cutoff)
                        throw new OldNoteDeleteException("You cannot delete a note older than 2 days.");

                    userIdToInvalidate = note.UserId;

                    await _unitOfWork.DiaryRecordRepository.DeleteAsync(note);
                    break;
                }

                case NoteType.Image:
                {
                    var image = await _unitOfWork.DiaryImageRepository.GetByIDAsync(deleteRequest.NoteId);
                    if (image is null)
                        throw new ArgumentException("Image note not found.", nameof(deleteRequest.NoteId));

                    if (image.UploadedAt < cutoff)
                        throw new OldNoteDeleteException("You cannot delete a note older than 2 days.");

                    userIdToInvalidate = image.UserId;

                    await _unitOfWork.DiaryImageRepository.DeleteAsync(image);
                    break;
                }

                default:
                    throw new ArgumentException("Invalid note type.", nameof(deleteRequest.NoteType));
            }

            await _unitOfWork.SaveAsync();

            InvalidateUserNotesCache(userIdToInvalidate);

            return deleteRequest.NoteId;
        }

        private string CacheKey(long userId) => _userNotesKeyPattern.Replace("{userId}", userId.ToString());

        private void InvalidateUserNotesCache(long userId) => _cache.Remove(CacheKey(userId));

        /// <summary>
        /// Converts images to WebP with a resizing to 1920x1080 (max) and the specified quality.
        /// Returns bytes and content-type "image/webp".
        /// </summary>
        private async Task<(byte[] data, string contentType)> OptimizeImageAsync(byte[] input, int quality = 75)
        {
            try
            {
                using var msInput = new MemoryStream(input);
                using var image = await Image.LoadAsync(msInput);
                if (image.Width > 1920 || image.Height > 1080)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(1920, 1080)
                    }));
                }

                image.Metadata.ExifProfile = null;
                image.Metadata.IccProfile = null;

                using var msOutput = new MemoryStream();
                var encoder = new WebpEncoder
                {
                    Quality = quality,
                    FileFormat = WebpFileFormatType.Lossy
                };

                await image.SaveAsync(msOutput, encoder);
                return (msOutput.ToArray(), "image/webp");
            }
            catch
            {
                return (input, "application/octet-stream");
            }
        }
    }
}