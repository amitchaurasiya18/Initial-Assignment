using CoreServices.GenericRepository.Interfaces;
using MediatR;
using SchoolAPI.Business.Models;
using SchoolAPI.Commands;
using SchoolAPI.StaticFiles;

namespace SchoolAPI.Handlers
{
    public class UpdateStudentHandler : IRequestHandler<UpdateStudentCommand, Student>
    {
        private readonly IWriteRepository<Student> _writeRepository;
        private readonly IReadRepository<Student> _readRepository;

        public UpdateStudentHandler(IWriteRepository<Student> writeRepository, IReadRepository<Student> readRepository)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
        }

        public async Task<Student> Handle(UpdateStudentCommand command, CancellationToken cancellationToken)
        {
            var existingStudent = await _readRepository.GetById(command.Student.Id);
            if (existingStudent == null)
            {
                throw new KeyNotFoundException(ErrorMessages.STUDENT_NOT_FOUND);
            }

            UpdateProperties(existingStudent, command.Student);

            existingStudent.UpdatedAt = DateTime.UtcNow;
            return await _writeRepository.Update(existingStudent);
        }

        private void UpdateProperties(Student existingStudent, Student updatedStudent)
        {
            existingStudent.FirstName = updatedStudent.FirstName ?? existingStudent.FirstName;
            existingStudent.LastName = updatedStudent.LastName ?? existingStudent.LastName;
            existingStudent.Email = updatedStudent.Email ?? existingStudent.Email;
            existingStudent.Phone = updatedStudent.Phone ?? existingStudent.Phone;
            existingStudent.DateOfBirth = updatedStudent.DateOfBirth ?? existingStudent.DateOfBirth;
        }
    }
}
