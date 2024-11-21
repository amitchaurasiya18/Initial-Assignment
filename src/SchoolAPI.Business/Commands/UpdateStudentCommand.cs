using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Services.Interfaces;

namespace SchoolAPI.Business.Commands
{
    public class UpdateStudentCommand : IRequest<Student>
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        public UpdateStudentCommand(int Id, string FirstName, string LastName, string Email, string Phone, DateTime DateOfBirth)
        {
            this.Id = Id;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.Phone = Phone;
            this.DateOfBirth = DateOfBirth;
        }
    }
}