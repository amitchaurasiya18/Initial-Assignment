using SchoolAPI.DTO;
using SchoolAPI.Models;

namespace SchoolAPI.Services.Interfaces
{
    public interface IStudentService
    {
        StudentPostDTO Add(StudentPostDTO studentDTO);
        IEnumerable<StudentGetDTO> GetAll();
        Task<Student> Update(int id, StudentPostDTO studentPostDTO);
        Task<string> Delete(int id);
        StudentGetDTO GetById(int id);
        Task<object> FilterStudents(int page, int pageSize, string searchTerm);
    }
}
