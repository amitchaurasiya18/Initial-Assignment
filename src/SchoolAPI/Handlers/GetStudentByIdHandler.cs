using CoreServices.GenericRepository.Interfaces;
using MediatR;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Repository.Interfaces;
using SchoolAPI.Queries;

namespace SchoolAPI.Business.Handlers
{
    public class GetStudentByIdHandler : IRequestHandler<GetStudentByIdQuery, Student>
    {
        private readonly IReadRepository<Student> _readRepository;

        public GetStudentByIdHandler(IReadRepository<Student> readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<Student> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            return await _readRepository.GetById(request.Id);
        }
    }
}