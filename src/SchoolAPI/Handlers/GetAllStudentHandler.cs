using CoreServices.GenericRepository.Interfaces;
using MediatR;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Repository.Interfaces;
using SchoolAPI.Queries;

namespace SchoolAPI.Handlers
{
    public class GetAllStudentHandler : IRequestHandler<GetAllStudentQuery, IEnumerable<Student>>
    {
        private readonly IReadRepository<Student> _readRepository;

        public GetAllStudentHandler(IReadRepository<Student> readRepository)
        {
            _readRepository = readRepository;
        }
        
        public async Task<IEnumerable<Student>> Handle(GetAllStudentQuery request, CancellationToken cancellationToken)
        {
            return await _readRepository.GetAll();
        }
    }
}