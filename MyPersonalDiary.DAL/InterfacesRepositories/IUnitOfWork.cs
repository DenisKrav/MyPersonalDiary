using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.DAL.InterfacesRepositories
{
    public interface IUnitOfWork
    {
        IInviteRepository InviteRepository { get; }
        public IDiaryImageRepository DiaryImageRepository { get; }
        public IDiaryRecordRepository DiaryRecordRepository { get; }

        Task SaveAsync();
    }
}
