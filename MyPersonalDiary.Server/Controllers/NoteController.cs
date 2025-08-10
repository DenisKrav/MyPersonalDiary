using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.BLL.DTOs.Note.Request;
using MyPersonalDiary.BLL.Exceptions;
using MyPersonalDiary.BLL.InterfacesServices;
using MyPersonalDiary.Server.Utilities;
using MyPersonalDiary.Server.ViewModels.Diary.Request;
using MyPersonalDiary.Server.ViewModels.Note.Request;
using System.Security.Claims;

namespace MyPersonalDiary.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _noteService;
        private readonly IMapper _mapper;

        public NoteController(INoteService noteService, IMapper mapper)
        {
            _noteService = noteService;
            _mapper = mapper;

        }

        [HttpGet("GetUserNotes")]
        public async Task<GeneralResultModel> GetUserNotes()
        {
            var result = new GeneralResultModel();
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!long.TryParse(userIdClaim, out var userId))
                {
                    result.Errors.Add("Unauthorized");
                    Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return result;
                }

                result.Result = await _noteService.GetUserNotesAsync(userId);
                return result;
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return result;
            }
        }

        [HttpPost("AddNote")]
        public async Task<GeneralResultModel> AddNote([FromForm] AddNoteRequestViewModel addRequest)
        {
            var result = new GeneralResultModel();

            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!long.TryParse(userIdClaim, out var userId))
                {
                    result.Errors.Add("Unauthorized");
                    Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return result;
                }

                byte[]? imageBytes = null;
                string? imageContentType = null;
                if (addRequest.Image != null && addRequest.Image.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await addRequest.Image.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                    imageContentType = addRequest.Image.ContentType;
                }

                var addNoteDto = _mapper.Map<AddNoteRequestDto>(addRequest);
                addNoteDto.UserId = userId;
                addNoteDto.ImageData = imageBytes;
                addNoteDto.ImageContentType = imageContentType;


                result.Result = await _noteService.AddNoteAsync(addNoteDto);

                return result;
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return result;
            }
        }

        [HttpDelete("DeleteNote")]
        public async Task<GeneralResultModel> DeleteNote([FromBody] DeleteNoteRequestViewModel deleteRequest)
        {
            var result = new GeneralResultModel();

            try
            {
                var deleteRequestDto = _mapper.Map<DeleteNoteRequestDto>(deleteRequest);
                result.Result = await _noteService.DeleteNoteAsync(deleteRequestDto);

                return result;
            }
            catch (OldNoteDeleteException ex)
            {
                result.Errors.Add("Note older than two days.");
                result.Errors.Add(ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return result;
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return result;
            }
        }
    }
}
