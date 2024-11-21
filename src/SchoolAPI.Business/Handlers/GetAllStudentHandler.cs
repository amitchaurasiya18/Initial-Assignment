using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Queries;
using SchoolAPI.Business.Repository.Interfaces;

namespace SchoolAPI.Business.Handlers
{
    public class GetAllStudentHandler : IRequestHandler<GetAllStudentQuery, IEnumerable<Student>>
    {
        private readonly IStudentRepository _studentRepository;

        public GetAllStudentHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }
        
        public async Task<IEnumerable<Student>> Handle(GetAllStudentQuery request, CancellationToken cancellationToken)
        {
            return await _studentRepository.GetAll();
        }
    }
}