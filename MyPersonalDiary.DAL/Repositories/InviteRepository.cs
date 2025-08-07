using MyPersonalDiary.DAL.ApplicationDbContext;
using MyPersonalDiary.DAL.InterfacesRepositories;
using MyPersonalDiary.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.DAL.Repositories
{
    public class InviteRepository: GenericRepository<Invite>, IInviteRepository
    {
        public InviteRepository(AppDbContext context) : base(context)
        {
        }
    }
}
