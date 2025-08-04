using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.DAL.Models.Identities
{
    public class ApplicationUser: IdentityUser<long>
    {
        public string NickName { get; set; }
        public bool IsPendingDeletion { get; set; } = false;
        public DateTime? DeletionRequestedAt { get; set; }
    }
}
