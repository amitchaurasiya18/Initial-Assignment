using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SchoolAPI.Business.Models;

namespace SchoolAPI.Queries
{
    public class GetStudentByEmailQuery : IRequest<Student>
    {
        public string Email { get; set; } = string.Empty;
    }
}