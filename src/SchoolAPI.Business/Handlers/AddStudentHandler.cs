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

        public async Task<Student> Handle(AddStudentCommand command, CancellationToken cancellationToken)
        {
            command.Student.CreatedAt = DateTime.Now;
            command.Student.UpdatedAt = DateTime.Now;
            command.Student.IsActive = true;

            return await _studentRepository.Add(command.Student);
        }
    }
}