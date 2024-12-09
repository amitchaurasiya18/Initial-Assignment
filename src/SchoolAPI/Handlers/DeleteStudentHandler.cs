using CoreServices.GenericRepository.Interfaces;
using MediatR;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Repository.Interfaces;
using SchoolAPI.Commands;

namespace SchoolAPI.Handlers
{
    public class DeleteStudentHandler  :IRequestHandler<DeleteStudentCommand, bool>
    {
        private readonly IWriteRepository<Student> _writeRepository;
        private readonly IReadRepository<Student> _readRepository;

        public DeleteStudentHandler(IWriteRepository<Student> writeRepository, IReadRepository<Student> readRepository)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
        }

        public async Task<bool> Handle(DeleteStudentCommand command, CancellationToken cancellationToken)
        {
            var student = await _readRepository.GetById(command.Id);

            return await _writeRepository.Delete(student.Id);
        }
    }
}