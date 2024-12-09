using CoreServices.GenericRepository.Interfaces;
using MediatR;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Repository.Interfaces;
using SchoolAPI.Commands;

namespace SchoolAPI.Handlers
{
    public class AddStudentHandler : IRequestHandler<AddStudentCommand, Student>
    {
        private readonly IWriteRepository<Student> _writeRepository;

        public AddStudentHandler(IWriteRepository<Student> writeRepository)
        {
            _writeRepository = writeRepository;
        }

        public async Task<Student> Handle(AddStudentCommand command, CancellationToken cancellationToken)
        {
            command.Student.CreatedAt = DateTime.Now;
            command.Student.UpdatedAt = DateTime.Now;
            command.Student.IsActive = true;

            return await _writeRepository.Add(command.Student);
        }
    }
}