using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.InterfacesServices
{
    public interface IInviteService
    {
        Task<string> SendInviteAsync(string email);
    }
}
