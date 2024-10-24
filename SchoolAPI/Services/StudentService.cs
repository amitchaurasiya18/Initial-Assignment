using AutoMapper;
using SchoolAPI.DTO;
using SchoolAPI.Models;
using SchoolAPI.Repository.Interfaces;
using SchoolAPI.Services.Interfaces;

namespace SchoolAPI.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;

        public StudentService(IStudentRepository studentRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
        }

        public StudentPostDTO Add(StudentPostDTO studentDTO)
        {
            if(studentDTO == null)
            {
                throw new ArgumentNullException("Student Data is required.");
            }

            var student = _mapper.Map<Student>(studentDTO);
            student.CreatedAt = DateTime.Now;
            student.UpdatedAt = DateTime.Now;
            student.isActive = true;
            student.Age = CalculateAge(student.DateOfBirth);

            _studentRepository.Add(student);
            return studentDTO;
        }

        public IEnumerable<StudentGetDTO> GetAll()
        {
            var students =  _studentRepository.GetAll();

            IEnumerable<StudentGetDTO> newStudents = _mapper.Map<IEnumerable<StudentGetDTO>>(students);

            return newStudents;
        }

        public int CalculateAge(DateTime dateOfBirth)
        {
            DateTime today = DateTime.Now;
            int age = today.Year - dateOfBirth.Year;

            if (dateOfBirth.Year > today.Year)
            {
                age--;
            }

            return age;
        }

        public async Task<Student> Update(int id, StudentPostDTO studentPostDTO)
        {
            Student student = _studentRepository.GetById(id);
            student.FirstName = studentPostDTO.FirstName;
            student.LastName = studentPostDTO.LastName;
            student.Email = studentPostDTO.Email;
            student.Phone = studentPostDTO.Phone;
            student.DateOfBirth = studentPostDTO.DateOfBirth;
            student.Age = CalculateAge(studentPostDTO.DateOfBirth);
            student.UpdatedAt = DateTime.Now;

            await _studentRepository.Update(student);
            return student;
        }

        public async Task<string> Delete(int id)
        {
            return await _studentRepository.Delete(id); ;
        }

        public StudentGetDTO GetById(int id)
        {
            var student = _studentRepository.GetById(id);
            StudentGetDTO newStudent = _mapper.Map<StudentGetDTO>(student);
            return newStudent;
        }

        public async Task<object> FilterStudents(int page, int pageSize, string searchTerm)
        {
            var (students, totalcount) = await _studentRepository.FilterStudents(page, pageSize, searchTerm);

            var result = new
            {
                Students = _mapper.Map<IEnumerable<StudentGetDTO>>(students),
                TotalCount = totalcount
            };

            return result;
        }
    }
}
