using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Services.Interfaces;

namespace SchoolAPI.Business.Commands
{
    public class AddStudentCommand : IRequest<Student>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }

        public AddStudentCommand(string FirstName, string LastName, string Email, string Phone, DateTime DateOfBirth, int Age)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.Phone = Phone;
            this.DateOfBirth = DateOfBirth;
            this.Age = Age;
        }
    }
}