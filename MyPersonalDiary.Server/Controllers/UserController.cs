using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.BLL.DTOs.Note.Request;
using MyPersonalDiary.BLL.Exceptions;
using MyPersonalDiary.BLL.InterfacesServices;
using MyPersonalDiary.Server.Utilities;
using MyPersonalDiary.Server.ViewModels.Note.Request;
using MyPersonalDiary.Server.ViewModels.User.Request;
using System.Security.Claims;

namespace MyPersonalDiary.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "User")]
        [HttpDelete("DeleteUser")]
        public async Task<GeneralResultModel> DeleteUser()
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

                result.Result = await _userService.DeleteSoftUserAsync(userId);

                return result;
            }
            catch (UserExistExeption ex)
            {
                result.Errors.Add("User with this email already exists.");
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

        [HttpPost("RestoreAccount")]
        public async Task<GeneralResultModel> RestoreAccount([FromBody] RestoreAccountRequestViewModel request)
        {
            var result = new GeneralResultModel();
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    result.Errors.Add("Email cannot be empty.");
                    Response.StatusCode = StatusCodes.Status400BadRequest;
                    return result;
                }

                var user = await _userService.RestoreSoftDeletedUserAsync(request.Email);
                result.Result = user;
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
