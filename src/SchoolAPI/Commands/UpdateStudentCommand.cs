using MediatR;
using SchoolAPI.Business.Models;

namespace SchoolAPI.Commands
{
    public class UpdateStudentCommand : IRequest<Student>
    {
        public Student Student { get; set; }

        public UpdateStudentCommand(Student student)
        {
            this.Student = student;
        }
    }
}