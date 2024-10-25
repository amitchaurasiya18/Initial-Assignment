using SchoolAPI.Business.Models;
using SchoolAPI.Business.Repository.Interfaces;
using SchoolAPI.Business.Services.Interfaces;

namespace SchoolAPI.Business.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<Student> Add(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException("Student Data is required.");
            }
            var addedStudent = await _studentRepository.Add(student);
            return addedStudent;
        }

        public async Task<IEnumerable<Student>> GetAll()
        {
            var students = _studentRepository.GetAll();
            return await students;
        }

        public async Task<int> CalculateAge(DateTime dateOfBirth)
        {
            DateTime today = DateTime.Now;
            int age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Year > today.Year)
            {
                age--;
            }
            return age;
        }

        public async Task<Student> Update(int id, Student student)
        {
            var existingStudent = await _studentRepository.GetById(id);
            if (existingStudent == null)
            {
                throw new KeyNotFoundException("Student not found");
            }

            existingStudent.FirstName = student.FirstName;
            existingStudent.LastName = student.LastName;
            existingStudent.Email = student.Email;
            existingStudent.Phone = student.Phone;
            existingStudent.DateOfBirth = student.DateOfBirth;
            existingStudent.Age = await CalculateAge(student.DateOfBirth);
            existingStudent.UpdatedAt = DateTime.Now;

            return await _studentRepository.Update(existingStudent);
        }


        public async Task<string> Delete(int id)
        {
            return await _studentRepository.Delete(id); ;
        }

        public async Task<Student> GetById(int id)
        {
            var student = await _studentRepository.GetById(id);
            return student;
        }

        public async Task<FilteredStudent> FilterStudents(int page, int pageSize, string searchTerm)
        {
            var (students, totalCount) = await _studentRepository.FilterStudents(page, pageSize, searchTerm);

            var result = new FilteredStudent
            {
                Students = students,
                TotalCount = totalCount
            };
            return result;
        }
    }
}
