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
    public class AddStudentHandler : IRequestHandler<AddStudentCommand, Student>
    {
        private readonly IStudentRepository _studentRepository;

        public AddStudentHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public Task<Student> Handle(AddStudentCommand command, CancellationToken cancellationToken)
        {
            var student = new Student()
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                Phone = command.Phone,
                DateOfBirth = command.DateOfBirth,
                Age = command.Age,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                isActive = true
            };

            return _studentRepository.Add(student);
        }
    }
}