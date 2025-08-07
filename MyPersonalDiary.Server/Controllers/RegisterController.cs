using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.BLL.DTOs.User.Request;
using MyPersonalDiary.BLL.Exceptions;
using MyPersonalDiary.BLL.InterfacesServices;
using MyPersonalDiary.DAL.Enums;
using MyPersonalDiary.Server.Utilities;
using MyPersonalDiary.Server.ViewModels.User.Request;

namespace MyPersonalDiary.Server.Controllers
{
    [Route("api/register")]
    [ApiController]
    public class RegisterController: ControllerBase
    {
        private readonly IRegisterService _registeredServices;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public RegisterController(IRegisterService registeredServices, IAuthService authService, IMapper mapper, IUserService userService)
        {
            _registeredServices = registeredServices;
            _authService = authService;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpPost("RegisterUser")]
        public async Task<ActionResult<GeneralResultModel>> Register([FromBody] NewUserRequestViewModel request)
        {
            GeneralResultModel generalResult = new GeneralResultModel();
            try
            {
                // TODO: change logick for defining role
                request.Role = UserRole.User;
                var newUser = _mapper.Map<NewUserDTO>(request);
                await _registeredServices.RegisterUserAsync(newUser);

                var userDto = await _userService.GetUserByEmailAsync(request.Email);
                var token = await _authService.GenerateToken(userDto.Id.ToString(), userDto.Email);
                generalResult.Result = token;
                return Ok(generalResult);
            }
            catch (InvalidLoginPasswordException ex)
            {
                generalResult.Errors.Add(ex.Message);
                return Unauthorized(generalResult);
            }
            catch (InvalidLoginEmailException ex)
            {
                generalResult.Errors.Add(ex.Message);
                return Unauthorized(generalResult);
            }
            catch (UserArgumentException ex)
            {
                generalResult.Errors.Add(ex.Message);
                return Unauthorized(generalResult);
            }
            catch (Exception ex)
            {
                generalResult.Errors.Add(ex.Message);
                return StatusCode(500, generalResult);
            }
        }
    }
}
