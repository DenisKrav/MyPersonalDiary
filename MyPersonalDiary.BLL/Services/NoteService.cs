using AutoDependencyRegistration.Attributes;
using MyPersonalDiary.BLL.InterfacesServices;
using MyPersonalDiary.DAL.InterfacesRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.Services
{
    [RegisterClassAsTransient]
    public class NoteService: INoteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NoteService(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        public Task AddNoteAsync()
    }
}
