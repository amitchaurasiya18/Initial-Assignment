using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Retrieves all students.
        /// </summary>
        /// <returns>List of all students</returns>
        /// <response code="200">Returns the list of students</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="404">No students found</response>
        [HttpGet]
        [Authorize(Roles = "Admin, Teacher")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<StudentGetDTO>))]
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

        /// <summary>
        /// Retrieves a student by ID.
        /// </summary>
        /// <param name="id">The ID of the student</param>
        /// <returns>The student details</returns>
        /// <response code="200">Returns the student details</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="404">Student not found</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Teacher")]
        [ProducesResponseType(200, Type = typeof(StudentGetDTO))]
        public async Task<ActionResult<StudentGetDTO>> GetById(int id)
        {
            var student = await _studentRepository.GetById(id);
            if (student == null)
            {
                return NotFound(ErrorMessages.STUDENT_NOT_FOUND);
            }
            var studentDTO = _mapper.Map<StudentGetDTO>(student);
            return Ok(studentDTO);
        }

        /// <summary>
        /// Filters students based on search term, page number, and page size.
        /// </summary>
        /// <param name="page">Page number (default is 1)</param>
        /// <param name="pageSize">Page size (default is 10)</param>
        /// <param name="searchTerm">Search term to filter students</param>
        /// <returns>List of filtered students</returns>
        /// <response code="200">Returns filtered students</response>
        /// <response code="406">Non-acceptable Input for Page number or page size</response>
        /// <response code="401">Unauthorized request</response>
        [HttpGet("filter-student")]
        [Authorize(Roles = "Admin, Teacher")]
        [ProducesResponseType(200, Type = typeof(FilteredStudent))]
        public async Task<ActionResult<FilteredStudent>> FilterStudents(int page = PAGE, int pageSize = PAGE_SIZE, string searchTerm = SEARCH_TERM)
        {
            if(page < 0)
            {
                throw new InvalidPageNumber(ErrorMessages.INVALID_PAGE_NUMBER);
            }

            if(pageSize <= 0)
            {
                throw new InvalidPageSize(ErrorMessages.INVALID_PAGE_SIZE);
            }
            
            var result = await _studentRepository.FilterStudents(page, pageSize, searchTerm);
            var studentDTOs = _mapper.Map<IEnumerable<StudentGetDTO>>(result.Item1);
            return Ok(new FilteredStudent { Students = studentDTOs, TotalCount = result.TotalCount });
        }

        /// <summary>
        /// Adds a new student.
        /// </summary>
        /// <param name="studentPostDTO">The student data to create</param>
        /// <returns>Details of the created student</returns>
        /// <response code="200">Returns the created student</response>
        /// <response code="400">Invalid student data</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="409">Email already registered</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200, Type = typeof(StudentGetDTO))]
        public async Task<ActionResult<StudentGetDTO>> AddStudent([FromBody] StudentPostDTO studentPostDTO)
        {
            var student = _mapper.Map<Student>(studentPostDTO);

            var alreadyRegisteredStudent = await _studentRepository.GetByEmail(student.Email);

            if (alreadyRegisteredStudent != null)
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

        /// <summary>
        /// Updates an existing student's information.
        /// </summary>
        /// <param name="id">The ID of the student</param>
        /// <param name="studentUpdateDTO">The updated student data</param>
        /// <returns>The updated student details</returns>
        /// <response code="200">Returns the updated student details</response>
        /// <response code="400">Invalid data provided</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="404">Student not found</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200, Type = typeof(StudentGetDTO))]
        public async Task<ActionResult<StudentGetDTO>> UpdateStudent(int id, [FromBody] StudentUpdateDTO studentUpdateDTO)
        {
            var existingStudent = await _studentRepository.GetById(id);
            if (existingStudent == null)
            {
                return NotFound(ErrorMessages.STUDENT_NOT_FOUND);
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

        /// <summary>
        /// Deletes a student by ID.
        /// </summary>
        /// <param name="id">The ID of the student</param>
        /// <returns>The deleted student details</returns>
        /// <response code="200">Returns the deleted student details</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="404">Student not found</response>
        /// <response code="400">Error while deleting student</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200, Type = typeof(StudentGetDTO))]
        public async Task<ActionResult<StudentGetDTO>> DeleteStudent(int id)
        {
            var student = await _studentRepository.GetById(id);
            if (student == null)
            {
                return NotFound(ErrorMessages.STUDENT_NOT_FOUND);
            }
            var result = await _studentRepository.Delete(student.Id);
            if (!result)
            {
                return BadRequest(ErrorMessages.INTERNAL_SERVER_ERROR);
            }
            var studentDTO = _mapper.Map<StudentGetDTO>(student);
            return Ok(studentDTO);
        }
    }
}

