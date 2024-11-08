using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Repository.Interfaces;
using SchoolAPI.Business.Services.Interfaces;
using SchoolAPI.CustomExceptions;
using SchoolAPI.DTO;
using SchoolAPI.StaticFiles;

namespace SchoolAPI.Controllers
{
    [Route("[controller]")]
    [ServiceFilter(typeof(ModelValidationFilter))]
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

        [HttpGet]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<ActionResult<IEnumerable<StudentGetDTO>>> GetAllStudents()
        {
            var students = await _studentRepository.GetAll();

            if (students == null)
            {
                throw new NoStudentsFound(ErrorMessages.NO_STUDENTS_FOUND);
            }

            var studentDTOs = _mapper.Map<IEnumerable<StudentGetDTO>>(students);
            return Ok(studentDTOs);

        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<ActionResult<StudentGetDTO>> GetById(int id)
        {
            var student = await _studentRepository.GetById(id);
            if (student == null)
            {
                throw new StudentNotFound(ErrorMessages.STUDENT_NOT_FOUND);
            }
            var studentDTO = _mapper.Map<StudentGetDTO>(student);
            return Ok(studentDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<StudentGetDTO>> AddStudent([FromBody] StudentPostDTO studentPostDTO)
        {
            var student = _mapper.Map<Student>(studentPostDTO);

            var alreadyRegisteredStudent = await _studentRepository.GetByEmail(student.Email);

            if (alreadyRegisteredStudent!= null)
            {
                throw new EmailAlreadyRegistered(student.Email + ErrorMessages.EMAIL_ALREADY_REGISTERED);
            }

            student.CreatedAt = DateTime.Now;
            student.UpdatedAt = DateTime.Now;
            student.Age = await _studentService.CalculateAge(student.DateOfBirth);

            var addedStudent = await _studentRepository.Add(student);
            var addedStudentDTO = _mapper.Map<StudentGetDTO>(addedStudent);

            return Ok(addedStudentDTO);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<StudentGetDTO>> UpdateStudent(int id, [FromBody] StudentUpdateDTO studentUpdateDTO)
        {
            var existingStudent = await _studentRepository.GetById(id);
            if (existingStudent == null)
            {
                throw new StudentNotFound(ErrorMessages.STUDENT_NOT_FOUND);
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<StudentGetDTO>> DeleteStudent(int id)
        {
            var student = await _studentRepository.GetById(id);
            if (student == null)
            {
                throw new KeyNotFoundException(ErrorMessages.STUDENT_NOT_FOUND);
            }
            var result = await _studentRepository.Delete(student.Id);
            if (!result)
            {
                throw new BadHttpRequestException(ErrorMessages.INTERNAL_SERVER_ERROR);
            }
            var studentDTO = _mapper.Map<StudentGetDTO>(student);
            return Ok(studentDTO);
        }


        [HttpGet("filter-student")]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<ActionResult<FilteredStudent>> FilterStudents(int page = PAGE, int pageSize = PAGE_SIZE, string searchTerm = SEARCH_TERM)
        {
            var result = await _studentRepository.FilterStudents(page, pageSize, searchTerm);
            var studentDTOs = _mapper.Map<IEnumerable<StudentGetDTO>>(result.Item1);
            return Ok(new FilteredStudent { Students = studentDTOs, TotalCount = result.TotalCount });
        }
    }
}