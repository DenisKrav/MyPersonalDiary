using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.BLL.DTOs.Note.Request;
using MyPersonalDiary.BLL.InterfacesServices;
using MyPersonalDiary.Server.Utilities;
using MyPersonalDiary.Server.ViewModels.Diary.Request;
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

        [HttpPost("AddNote")]
        public async Task<GeneralResultModel> AddNote([FromForm] AddNoteRequestViewModel model)
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
                if (model.Image != null && model.Image.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await model.Image.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                    imageContentType = model.Image.ContentType;
                }

                var addNoteDto = _mapper.Map<AddNoteRequestDto>(model);
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
    }
}
