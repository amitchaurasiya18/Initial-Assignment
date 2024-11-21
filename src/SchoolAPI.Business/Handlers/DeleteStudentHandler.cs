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
    public class DeleteStudentHandler  :IRequestHandler<DeleteStudentCommand, bool>
    {
        private readonly IStudentRepository _studentRepository;

        public DeleteStudentHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<bool> Handle(DeleteStudentCommand command, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetById(command.Id);

            return await _studentRepository.Delete(student.Id);
        }
    }
}