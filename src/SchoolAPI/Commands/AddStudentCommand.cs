using MediatR;
using SchoolAPI.Business.Models;

namespace SchoolAPI.Commands
{
    public class AddStudentCommand : IRequest<Student>
    {
        public Student Student { get; set; }

        public AddStudentCommand(Student student)
        {
            this.Student = student;
        }
    }
}