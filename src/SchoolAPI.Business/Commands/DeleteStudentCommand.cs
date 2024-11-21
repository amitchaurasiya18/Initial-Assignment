using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SchoolAPI.Business.Models;

namespace SchoolAPI.Business.Commands
{
    public class DeleteStudentCommand :IRequest<bool>
    {
        public int Id { get; set; }
    }
}