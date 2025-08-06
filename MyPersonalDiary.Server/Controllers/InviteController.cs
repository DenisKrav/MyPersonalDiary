using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.BLL.Exceptions;
using MyPersonalDiary.BLL.InterfacesServices;
using MyPersonalDiary.Server.Utilities;
using MyPersonalDiary.Server.ViewModels.Invite.Request;

namespace MyPersonalDiary.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class InviteController : ControllerBase
    {
        private readonly IInviteService _inviteService;

        public InviteController(IInviteService inviteService)
        {
            _inviteService = inviteService;
        }

        [HttpPost("SendInvite")]
        public async Task<GeneralResultModel> SendInvite([FromBody] InviteRequestViewModel inviteRequestViewModel)
        {
            var result = new GeneralResultModel();

            try
            {
                result.Result = await _inviteService.SendInviteAsync(inviteRequestViewModel.Email);
                return result;
            }
            catch (SendEmailExeption ex)
            {
                result.Errors.Add(ex.Message);
                return result;
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
                return result;
            }
        }
    }
}
