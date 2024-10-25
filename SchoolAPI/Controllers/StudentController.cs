using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using SchoolAPI.DTO;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Services.Interfaces;
using SchoolAPI.Validators;
using AutoMapper;
using SchoolAPI.Business;

namespace SchoolAPI.Controllers
{
    [Route("student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        private const int PAGE = 1;
        private const int PAGE_SIZE = 10;
        private const string SEARCH_TERM = "";

        public StudentController(IStudentService studentService, IMapper mapper)
        {
            _studentService = studentService;
            _mapper = mapper;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllStudents()
        {
            try
            {
                var students = await _studentService.GetAll();
                var studentDTOs = _mapper.Map<IEnumerable<StudentGetDTO>>(students);
                return Ok(studentDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var student = await _studentService.GetById(id);
                if (student == null)
                {
                    return NotFound("Student not found");
                }
                var studentDTO = _mapper.Map<StudentGetDTO>(student);
                return Ok(studentDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddStudent([FromBody] StudentPostDTO studentPostDTO)
        {
            StudentValidator validationRules = new StudentValidator();
            ValidationResult results = validationRules.Validate(studentPostDTO);

            if (!results.IsValid)
            {
                foreach (var failure in results.Errors)
                {
                    return BadRequest($"Property {failure.PropertyName} failed validation. Error was: {failure.ErrorMessage}");
                }
            }

            try
            {
                var student = _mapper.Map<Student>(studentPostDTO);
                student.CreatedAt = DateTime.Now;
                student.UpdatedAt = DateTime.Now;
                student.Age = await _studentService.CalculateAge(student.DateOfBirth);
                student.isActive = true;

                var addedStudent = await _studentService.Add(student);
                var addedStudentDTO = _mapper.Map<StudentGetDTO>(addedStudent);
                return Ok(addedStudentDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentPostDTO studentPostDTO)
        {
            try
            {
                var existingStudent = await _studentService.GetById(id);
                if (existingStudent == null)
                {
                    return NotFound("Student not found");
                }

                existingStudent.FirstName = studentPostDTO.FirstName;
                existingStudent.LastName = studentPostDTO.LastName;
                existingStudent.Email = studentPostDTO.Email;
                existingStudent.Phone = studentPostDTO.Phone;
                existingStudent.DateOfBirth = studentPostDTO.DateOfBirth;
                existingStudent.Age = await _studentService.CalculateAge(studentPostDTO.DateOfBirth);
                existingStudent.UpdatedAt = DateTime.Now;

                var updatedStudent = await _studentService.Update(id, existingStudent);
                var updatedStudentDTO = _mapper.Map<StudentGetDTO>(updatedStudent);
                return Ok(updatedStudentDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                var result = await _studentService.Delete(id);
                if (result == "Student not found")
                {
                    return NotFound("Student not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("filter-student")]
        public async Task<IActionResult> FilterStudents(int page = PAGE, int pageSize = PAGE_SIZE, string searchTerm = SEARCH_TERM)
        {
            try
            {
                var result = await _studentService.FilterStudents(page, pageSize, searchTerm);
                var studentDTOs = _mapper.Map<IEnumerable<StudentGetDTO>>(result.Students);
                return Ok((studentDTOs, studentDTOs.Count()));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
