using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using MyPersonalDiary.BLL.DTOs.Note.Request;
using MyPersonalDiary.BLL.DTOs.Note.Response;
using MyPersonalDiary.BLL.Exceptions;
using MyPersonalDiary.BLL.Services;
using MyPersonalDiary.DAL.InterfacesRepositories;
using MyPersonalDiary.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace MyPersonalDiaryNoteServise.Test
{
    public class NoteServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IDataProtector> _protectorMock = new();
        private readonly Mock<IDataProtectionProvider> _protectorProviderMock = new();
        private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        private readonly IConfiguration _config;

        private readonly Mock<IDiaryRecordRepository> _recordRepoMock = new();
        private readonly Mock<IDiaryImageRepository> _imageRepoMock = new();

        public NoteServiceTests()
        {
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string,string>("Protector:ContentProtector", "test"),
                    new KeyValuePair<string,string>("Cache:Keys:UserNotes", "notes:{userId}"),
                    new KeyValuePair<string,string>("Cache:TTL:AbsoluteMinutes", "5"),
                    new KeyValuePair<string,string>("Cache:TTL:SlidingMinutes", "2"),
                })
                .Build();

            _protectorProviderMock
                .Setup(p => p.CreateProtector(It.IsAny<string>()))
                .Returns(_protectorMock.Object);

            _uowMock.SetupGet(u => u.DiaryRecordRepository).Returns(_recordRepoMock.Object);
            _uowMock.SetupGet(u => u.DiaryImageRepository).Returns(_imageRepoMock.Object);
        }

        private NoteService CreateSut() => new(
            _uowMock.Object,
            _mapperMock.Object,
            _protectorProviderMock.Object,
            _config,
            _cache
        );

        [Fact]
        public async Task AddNoteAsync_Should_Add_Text_Note()
        {
            // Arrange
            var request = new AddNoteRequestDto 
            { 
                UserId = 1, 
                Content = "Hello Test" 
            };

            _protectorMock
                .Setup(p => p.Protect(It.IsAny<byte[]>()))
                .Returns<byte[]>(b => b);

            _recordRepoMock
                .Setup(r => r.InsertAsync(It.IsAny<DiaryRecord>()))
                .Returns(Task.CompletedTask);

            _uowMock.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            _mapperMock
                .Setup(m => m.Map<NoteResponseDto>(It.IsAny<DiaryRecord>()))
                .Returns(new NoteResponseDto { Content = "Hello Test" });

            var sut = CreateSut();

            // Act
            var result = await sut.AddNoteAsync(request);

            // Assert
            result.Content.Should().Be("Hello Test");
            _recordRepoMock.Verify(r => r.InsertAsync(It.IsAny<DiaryRecord>()), Times.Once);
            _uowMock.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task GetUserNotesAsync_Should_Return_FromCache_On_Second_Call()
        {
            // Arrange
            long userId = 10;

            var recs = new List<DiaryRecord>
            {
                new DiaryRecord { 
                    Id = Guid.NewGuid(), 
                    UserId = userId, 
                    EncryptedContent = "enc1",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-1) 
                },
            };
            var imgs = new List<DiaryImage>
            {
                new DiaryImage { 
                    Id = Guid.NewGuid(), 
                    UserId = userId,
                    UploadedAt = DateTime.UtcNow.AddMinutes(-2), 
                    Data = Array.Empty<byte>(), 
                    ContentType = "image/webp", 
                    FileName = "f.webp", 
                    Size = 1 
                },
            };

            _recordRepoMock
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<DiaryRecord, bool>>>(),
                    It.IsAny<Func<IQueryable<DiaryRecord>, IOrderedQueryable<DiaryRecord>>>(),
                    It.IsAny<string>()))
                .ReturnsAsync(recs);

            _imageRepoMock
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<DiaryImage, bool>>>(),
                    It.IsAny<Func<IQueryable<DiaryImage>, IOrderedQueryable<DiaryImage>>>(),
                    It.IsAny<string>()))
                .ReturnsAsync(imgs);

            _mapperMock
                .Setup(m => m.Map<NoteResponseDto>(It.IsAny<DiaryRecord>()))
                .Returns((DiaryRecord r) => new NoteResponseDto { Content = "[text]", CreatedAt = r.CreatedAt });

            _mapperMock
                .Setup(m => m.Map<NoteResponseDto>(It.IsAny<DiaryImage>()))
                .Returns((DiaryImage i) => new NoteResponseDto { ImageContentType = i.ContentType, CreatedAt = i.UploadedAt });

            var sut = CreateSut();

            // Act
            var first = (await sut.GetUserNotesAsync(userId)).ToList();
            var second = (await sut.GetUserNotesAsync(userId)).ToList();

            first.Should().HaveCount(2);
            second.Should().HaveCount(2);

            _recordRepoMock.Verify(r => r.GetAsync(
                    It.IsAny<Expression<Func<DiaryRecord, bool>>>(),
                    It.IsAny<Func<IQueryable<DiaryRecord>, IOrderedQueryable<DiaryRecord>>>(),
                    It.IsAny<string>()),
                Times.Once);

            _imageRepoMock.Verify(r => r.GetAsync(
                    It.IsAny<Expression<Func<DiaryImage, bool>>>(),
                    It.IsAny<Func<IQueryable<DiaryImage>, IOrderedQueryable<DiaryImage>>>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteNoteAsync_Text_YoungerThan2Days_Should_Delete_And_InvalidateCache()
        {
            // Arrange
            var userId = 5L;
            var noteId = Guid.NewGuid();
            var note = new DiaryRecord
            {
                Id = noteId,
                UserId = userId,
                EncryptedContent = "enc",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };

            var cacheKey = "notes:" + userId;
            _cache.Set(cacheKey, new[] { new NoteResponseDto() });

            _recordRepoMock.Setup(r => r.GetByIDAsync(noteId)).ReturnsAsync(note);
            _recordRepoMock.Setup(r => r.DeleteAsync(note)).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            var sut = CreateSut();

            // Act
            var deletedId = await sut.DeleteNoteAsync(new DeleteNoteRequestDto
            {
                NoteId = noteId,
                NoteType = "Text"
            });

            // Assert
            deletedId.Should().Be(noteId);
            _recordRepoMock.Verify(r => r.DeleteAsync(note), Times.Once);
            _uowMock.Verify(u => u.SaveAsync(), Times.Once);

            _cache.TryGetValue(cacheKey, out _).Should().BeFalse();
        }

        [Fact]
        public async Task DeleteNoteAsync_Text_OlderThan2Days_Should_Throw()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            var note = new DiaryRecord
            {
                Id = noteId,
                UserId = 7,
                EncryptedContent = "enc",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            };

            _recordRepoMock.Setup(r => r.GetByIDAsync(noteId)).ReturnsAsync(note);

            var sut = CreateSut();

            // Act
            var act = async () => await sut.DeleteNoteAsync(new DeleteNoteRequestDto
            {
                NoteId = noteId,
                NoteType = "Text"
            });

            // Assert
            await act.Should().ThrowAsync<OldNoteDeleteException>();
            _recordRepoMock.Verify(r => r.DeleteAsync(It.IsAny<DiaryRecord>()), Times.Never);
            _uowMock.Verify(u => u.SaveAsync(), Times.Never);
        }
    }
}
