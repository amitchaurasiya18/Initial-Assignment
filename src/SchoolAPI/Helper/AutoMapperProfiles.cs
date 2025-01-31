﻿using AutoMapper;
using SchoolAPI.DTO;
using SchoolAPI.Business.Models;


namespace SchoolAPI.Helper
{
    public class AutoMapperProfiles :Profile
    {
        public AutoMapperProfiles() 
        {
            CreateMap<Student, StudentPostDTO>().ReverseMap();
            CreateMap<Student, StudentGetDTO>().ReverseMap();
            CreateMap<StudentUpdateDTO, Student>().ReverseMap();
        }
    }
}
