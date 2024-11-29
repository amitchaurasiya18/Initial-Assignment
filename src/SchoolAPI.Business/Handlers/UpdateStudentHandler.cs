using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SchoolAPI.Business.Commands;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Repository.Interfaces;

namespace SchoolAPI.Business.Handlers
{
    public class UpdateStudentHandler : IRequestHandler<UpdateStudentCommand, Student>
    {
        private readonly IStudentRepository _studentRepository;

        public UpdateStudentHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<Student> Handle(UpdateStudentCommand command, CancellationToken cancellationToken)
        {
            var existingStudent = await _studentRepository.GetById(command.Student.Id);

            existingStudent.FirstName = command.Student.FirstName ?? existingStudent.FirstName;
            existingStudent.LastName = command.Student.LastName ?? existingStudent.LastName;
            existingStudent.Email = command.Student.Email ?? existingStudent.Email;
            existingStudent.Phone = command.Student.Phone ?? existingStudent.Phone;
            existingStudent.DateOfBirth = command.Student.DateOfBirth ?? existingStudent.DateOfBirth;
            existingStudent.UpdatedAt = DateTime.Now;

            return await _studentRepository.Update(existingStudent);
        }
    }
}
