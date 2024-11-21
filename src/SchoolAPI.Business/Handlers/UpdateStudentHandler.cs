using System;
using System.Collections.Generic;
using System.Linq;
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

            var student = await _studentRepository.GetById(command.Id);

            student.FirstName = command.FirstName;
            student.LastName = command.LastName;
            student.Email = command.Email;
            student.Phone = command.Phone;
            student.DateOfBirth = (DateTime)command.DateOfBirth;

            return await _studentRepository.Update(student);
        }
    }
}