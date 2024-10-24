using SchoolAPI.DTO;
using SchoolAPI.Models;

namespace SchoolAPI.Repository.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student> Add(Student student);
        IEnumerable<Student> GetAll();
        Student GetById(int id);
        Task<Student> Update(Student student);
        Task<string> Delete(int id);
        Task<(IEnumerable<Student>, int TotalCount)> FilterStudents(int page, int pageSize, string searchTerm); 
    }
}
