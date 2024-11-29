using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Services.Interfaces;

namespace SchoolAPI.Business.Commands
{
    public class AddStudentCommand : IRequest<Student>
    {
        public Student Student { get; set; }

        public AddStudentCommand(Student student)
        {
            this.Student = student;
        }
    }
}