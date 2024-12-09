using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreServices.GenericRepository.Interfaces;
using MediatR;
using SchoolAPI.Business.Models;
using SchoolAPI.Queries;

namespace SchoolAPI.Handlers
{
    public class GetStudentByEmailHandler: IRequestHandler<GetStudentByEmailQuery, Student>
    {
        private readonly IReadRepository<Student> _readRepository;

        public GetStudentByEmailHandler(IReadRepository<Student> readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<Student> Handle(GetStudentByEmailQuery request, CancellationToken cancellationToken)
        {
            return await _readRepository.GetByEmail(s=> s.Email == request.Email);
        }
    }
}