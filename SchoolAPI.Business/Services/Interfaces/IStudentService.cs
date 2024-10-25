using SchoolAPI.Business.Models;

namespace SchoolAPI.Business.Services.Interfaces
{
    public interface IStudentService
    {
        Task<Student> Add(Student student);
        Task<IEnumerable<Student>> GetAll();
        Task<Student> Update(int id, Student student);
        Task<string> Delete(int id);
        Task<Student> GetById(int id);
        Task<int> CalculateAge(DateTime dateOfBirth);
        Task<FilteredStudent> FilterStudents(int page, int pageSize, string searchTerm);
    }
}
