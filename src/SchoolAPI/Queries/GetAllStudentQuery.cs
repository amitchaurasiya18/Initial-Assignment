using MediatR;
using SchoolAPI.Business.Models;

namespace SchoolAPI.Queries
{
    public class GetAllStudentQuery : IRequest<IEnumerable<Student>>
    {
        
    }
}