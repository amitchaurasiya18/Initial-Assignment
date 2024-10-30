using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using SchoolAPI.DTO;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Services.Interfaces;
using SchoolAPI.Validators;
using AutoMapper;
using SchoolAPI.Business.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SchoolAPI.StaticFiles;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SchoolAPI.Controllers
{
    [Route("student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        private const int PAGE = 1;
        private const int PAGE_SIZE = 10;
        private const string SEARCH_TERM = "";

        public StudentController(IStudentRepository studentRepository, IMapper mapper, IStudentService studentService)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
            _studentService = studentService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _studentRepository.GetAll();

            if (students == null)
            {
                throw new KeyNotFoundException(ErrorMessages.STUDENT_NOT_FOUND);
            }

            var studentDTOs = _mapper.Map<IEnumerable<StudentGetDTO>>(students);
            return Ok(studentDTOs);

        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {

            var student = await _studentRepository.GetById(id);
            if (student == null)
            {
                throw new KeyNotFoundException(ErrorMessages.STUDENT_NOT_FOUND);
            }
            var studentDTO = _mapper.Map<StudentGetDTO>(student);
            return Ok(studentDTO);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddStudent([FromBody] StudentPostDTO studentPostDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            var student = _mapper.Map<Student>(studentPostDTO);
            student.CreatedAt = DateTime.Now;
            student.UpdatedAt = DateTime.Now;
            student.Age = await _studentService.CalculateAge(student.DateOfBirth);
            student.isActive = true;

            var addedStudent = await _studentRepository.Add(student);
            var addedStudentDTO = _mapper.Map<StudentGetDTO>(addedStudent);

            return Ok(addedStudentDTO);
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentUpdateDTO studentUpdateDTO)
        {
            var existingStudent = await _studentRepository.GetById(id);
            if (existingStudent == null)
            {
                throw new KeyNotFoundException(ErrorMessages.STUDENT_NOT_FOUND);
            }

            if (!string.IsNullOrEmpty(studentUpdateDTO.FirstName))
            {
                existingStudent.FirstName = studentUpdateDTO.FirstName;
            }

            if (!string.IsNullOrEmpty(studentUpdateDTO.LastName))
            {
                existingStudent.LastName = studentUpdateDTO.LastName;
            }

            if (!string.IsNullOrEmpty(studentUpdateDTO.Email))
            {
                existingStudent.Email = studentUpdateDTO.Email;
            }

            if (!string.IsNullOrEmpty(studentUpdateDTO.Phone))
            {
                existingStudent.Phone = studentUpdateDTO.Phone;
            }

            if (studentUpdateDTO.DateOfBirth != null)
            {
                existingStudent.DateOfBirth = (DateTime)studentUpdateDTO.DateOfBirth;
                existingStudent.Age = await _studentService.CalculateAge((DateTime)studentUpdateDTO.DateOfBirth);
            }

            existingStudent.UpdatedAt = DateTime.Now;

            var updatedStudent = await _studentRepository.Update(existingStudent);
            var updatedStudentDTO = _mapper.Map<StudentGetDTO>(updatedStudent);
            return Ok(updatedStudentDTO);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _studentRepository.GetById(id);
            if (student == null)
            {
                throw new KeyNotFoundException(ErrorMessages.STUDENT_NOT_FOUND);
            }
            var result = await _studentRepository.Delete(student.Id);
            if (!result)
            {
                throw new Exception(ErrorMessages.INTERNAL_SERVER_ERROR);
            }
            var studentDTO = _mapper.Map<StudentGetDTO>(student);
            return Ok(studentDTO);
        }


        [HttpGet("filter-student")]
        public async Task<IActionResult> FilterStudents(int page = PAGE, int pageSize = PAGE_SIZE, string searchTerm = SEARCH_TERM)
        {
            var result = await _studentRepository.FilterStudents(page, pageSize, searchTerm);
            var studentDTOs = _mapper.Map<IEnumerable<StudentGetDTO>>(result.Item1);
            return Ok(new FilteredStudent { Students = studentDTOs, TotalCount = result.TotalCount });
        }
    }
}
