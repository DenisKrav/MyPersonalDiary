using AutoMapper;
using MyPersonalDiary.BLL.DTOs.Note.Response;
using MyPersonalDiary.BLL.DTOs.User.Request;
using MyPersonalDiary.BLL.DTOs.User.Response;
using MyPersonalDiary.DAL.Models;
using MyPersonalDiary.DAL.Models.Identities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.Mappers
{
    public class NoteDTOProfile: Profile
    {
        public NoteDTOProfile() 
        {
            CreateMap<DiaryRecord, NoteResponseDto>();
            CreateMap<DiaryImage, NoteResponseDto>();
        }
    }
}
