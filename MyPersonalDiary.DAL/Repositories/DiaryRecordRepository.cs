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
    public class DiaryRecordRepository : GenericRepository<DiaryRecord>, IDiaryRecordRepository
    {
        public DiaryRecordRepository(AppDbContext context) : base(context)
        {
        }
    }
}
