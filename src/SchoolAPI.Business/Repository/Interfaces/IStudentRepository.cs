using SchoolAPI.Business.Models;

namespace SchoolAPI.Business.Repository.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student> Add(Student student);
        Task<IEnumerable<Student>> GetAll();
        Task<Student?> GetById(int id);
        Task<Student> Update(Student student);
        Task<bool> Delete(int id);
        Task<(IEnumerable<Student>, int TotalCount)> FilterStudents(int page, int pageSize, string searchTerm); 
        Task<Student> GetByEmail(string email);
    }
}
