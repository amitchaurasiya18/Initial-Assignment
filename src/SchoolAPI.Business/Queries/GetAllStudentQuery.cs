using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SchoolAPI.Business.Models;

namespace SchoolAPI.Business.Queries
{
    public class GetAllStudentQuery : IRequest<IEnumerable<Student>>
    {
        
    }
}