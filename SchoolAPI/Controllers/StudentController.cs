using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using SchoolAPI.DTO;
using SchoolAPI.Models;
using SchoolAPI.Services.Interfaces;
using SchoolAPI.Validators;
using System;

namespace SchoolAPI.Controllers
{
    [Route("student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private const int PAGE = 1;
        private const int PAGE_SIZE = 10;
        private const string SEARCH_TERM = "";

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet("get-all")]
        public IActionResult GetAllStudents()
        {
            try
            {
                var students = _studentService.GetAll();
                return Ok(students);
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
                var student = _studentService.GetById(id);
                if (student == null)
                {
                    return NotFound("Student not found");
                }
                return Ok(student);
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
                _studentService.Add(studentPostDTO);
                return Ok(studentPostDTO);
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
                var existingStudent = _studentService.GetById(id);
                if (existingStudent == null)
                {
                    return NotFound("Student not found");
                }

                await _studentService.Update(id, studentPostDTO);
                return Ok(studentPostDTO);
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
                if (result == null)
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
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
