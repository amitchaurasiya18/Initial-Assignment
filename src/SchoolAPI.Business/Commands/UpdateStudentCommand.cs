using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Services.Interfaces;

namespace SchoolAPI.Business.Commands
{
    public class UpdateStudentCommand : IRequest<Student>
    {
        public Student Student { get; set; }

        public UpdateStudentCommand(Student student)
        {
            this.Student = student;
        }
    }
}