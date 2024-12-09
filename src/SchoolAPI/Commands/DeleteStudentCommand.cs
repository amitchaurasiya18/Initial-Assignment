using MediatR;

namespace SchoolAPI.Commands
{
    public class DeleteStudentCommand :IRequest<bool>
    {
        public int Id { get; set; }
    }
}