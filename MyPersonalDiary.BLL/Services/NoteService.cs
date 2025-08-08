using AutoDependencyRegistration.Attributes;
using AutoMapper;
using Microsoft.AspNetCore.WebUtilities;
using MyPersonalDiary.BLL.DTOs.Note.Request;
using MyPersonalDiary.BLL.DTOs.Note.Response;
using MyPersonalDiary.BLL.InterfacesServices;
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
    public class NoteService: INoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        //private const int TenMb = 10 * 1024 * 1024;

        public NoteService(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<NoteResponseDto> AddNoteAsync(AddNoteRequestDto addNote)
        {
            if (addNote == null)
                throw new ArgumentNullException(nameof(addNote));

            var noteResponseDto = new NoteResponseDto();

            if (!string.IsNullOrWhiteSpace(addNote.Content))
            {
                if (addNote.Content.Length > 500)
                    throw new ArgumentException("Note content exceeds 500 characters limit.");

                var diaryRecord = new DiaryRecord
                {
                    Id = Guid.NewGuid(),
                    UserId = addNote.UserId,
                    EncryptedContent = addNote.Content,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.DiaryRecordRepository.InsertAsync(diaryRecord);
                await _unitOfWork.SaveAsync();

                return _mapper.Map<NoteResponseDto>(diaryRecord);
            }
            else if (addNote.ImageData != null && addNote.ImageData.Length > 0)
            {
                //byte[] finalImageData = addNote.ImageData;
                //string finalContentType = addNote.ImageContentType ?? "application/octet-stream";

                //if (finalImageData.Length > TenMb)
                //{
                //    (finalImageData, finalContentType) = await OptimizeImageAsync(
                //        finalImageData,
                //        finalContentType
                //    );
                //}

                var diaryImage = new DiaryImage
                {
                    Id = Guid.NewGuid(),
                    UserId = addNote.UserId,
                    FileName = $"note_{Guid.NewGuid()}",
                    ContentType = addNote.ImageContentType ?? "application/octet-stream",
                    //Size = finalImageData.Length,
                    //Data = finalImageData,
                    Size = addNote.ImageData.Length,
                    Data = addNote.ImageData,
                    UploadedAt = DateTime.UtcNow
                };

                await _unitOfWork.DiaryImageRepository.InsertAsync(diaryImage);
                await _unitOfWork.SaveAsync();

                return _mapper.Map<NoteResponseDto>(diaryImage);
            }
            else
            {
                throw new ArgumentException("Note must contain either text or an image.");
            }
        }

        //private async Task<(byte[] data, string contentType)> OptimizeImageAsync(byte[] input, string? originalContentType)
        //{
        //    using var msInput = new MemoryStream(input);

        //    // Завантажуємо без DecoderOptions — працює у будь-якій версії
        //    using var image = Image.Load(msInput, out IImageFormat _);

        //    if (image.Width > 1920 || image.Height > 1080)
        //    {
        //        image.Mutate(x => x.Resize(new ResizeOptions
        //        {
        //            Mode = ResizeMode.Max,
        //            Size = new Size(1920, 1080)
        //        }));
        //    }

        //    using var msOutput = new MemoryStream();
        //    var encoder = new WebpEncoder { Quality = 75 };
        //    await image.SaveAsync(msOutput, encoder);

        //    return (msOutput.ToArray(), "image/webp");
        //}


    }
}