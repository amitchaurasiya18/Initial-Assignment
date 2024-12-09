using AutoMapper;
using CoreServices.CustomExceptions;
using CoreServices.StaticFiles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Repository.Interfaces;
using SchoolAPI.Business.Services.Interfaces;
using SchoolAPI.Commands;
using SchoolAPI.DTO;
using SchoolAPI.Handlers;
using SchoolAPI.Queries;
using SchoolAPI.StaticFiles;

namespace SchoolAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class StudentCQRSController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private const int PAGE = 1;
        private const int PAGE_SIZE = 10;
        private const string SEARCH_TERM = "";

        public StudentCQRSController(IMapper mapper, IStudentService studentService, IMediator mediator)
        {
            _mapper = mapper;
            _studentService = studentService;
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves all students.
        /// </summary>
        /// <returns>List of all students</returns>
        /// <response code="200">Returns the list of students</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="404">No students found</response>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<StudentGetDTO>))]
        public async Task<ActionResult<IEnumerable<StudentGetDTO>>> GetAllStudents()
        {
            var students = await _mediator.Send(new GetAllStudentQuery());

            if (students == null || !students.Any())
            {
                return NotFound(ErrorMessages.NO_STUDENTS_FOUND);
            }

            var studentDTOs = _mapper.Map<IEnumerable<StudentGetDTO>>(students);
            foreach (var student in studentDTOs)
            {
                student.Age = _studentService.CalculateAge(student.DateOfBirth);
            }
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
        [ProducesResponseType(200, Type = typeof(StudentGetDTO))]
        public async Task<ActionResult<StudentGetDTO>> GetById(int id)
        {
            var student = await _mediator.Send(new GetStudentByIdQuery() { Id = id });
            if (student == null)
            {
                return NotFound(ErrorMessages.STUDENT_NOT_FOUND);
            }
            var studentDTO = _mapper.Map<StudentGetDTO>(student);
            studentDTO.Age = _studentService.CalculateAge(studentDTO.DateOfBirth);
            return Ok(studentDTO);
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
        [ProducesResponseType(200, Type = typeof(StudentGetDTO))]
        [Authorize(Roles = $"{AuthorizationRoles.ADMIN}")]
        public async Task<ActionResult<StudentGetDTO>> AddStudent([FromBody] StudentPostDTO studentPostDTO)
        {
            var student = _mapper.Map<Student>(studentPostDTO);
            var alreadyRegisteredStudent = await _mediator.Send(new GetStudentByEmailQuery() { Email = student.Email });

            if (alreadyRegisteredStudent != null)
            {
                throw new EmailAlreadyRegistered($"{student.Email} {ErrorMessages.EMAIL_ALREADY_REGISTERED}");
            }

            var addedStudent = await _mediator.Send(new AddStudentCommand(student));
            var addedStudentDTO = _mapper.Map<StudentGetDTO>(addedStudent);
            addedStudentDTO.Age = _studentService.CalculateAge((DateTime)addedStudentDTO.DateOfBirth);

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
        [ProducesResponseType(200, Type = typeof(StudentGetDTO))]
        [Authorize(Roles = $"{AuthorizationRoles.ADMIN}")]
        public async Task<ActionResult<StudentGetDTO>> UpdateStudent(int id, [FromBody] StudentUpdateDTO studentUpdateDTO)
        {
            var existingStudent = await _mediator.Send(new GetStudentByIdQuery() { Id = id });
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
            }

            existingStudent.UpdatedAt = DateTime.Now;

            var updatedStudent = await _mediator.Send(new UpdateStudentCommand(existingStudent));
            var updatedStudentDTO = _mapper.Map<StudentGetDTO>(updatedStudent);

            updatedStudentDTO.Age = _studentService.CalculateAge(updatedStudentDTO.DateOfBirth);

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
        [ProducesResponseType(200, Type = typeof(StudentGetDTO))]
        [Authorize(Roles = $"{AuthorizationRoles.ADMIN}")]
        public async Task<ActionResult<StudentGetDTO>> DeleteStudent(int id)
        {
            var student = await _mediator.Send(new GetStudentByIdQuery() { Id = id });
            if (student == null)
            {
                return NotFound(ErrorMessages.STUDENT_NOT_FOUND);
            }
            var result = await _mediator.Send(new DeleteStudentCommand() { Id = id });
            if (!result)
            {
                return BadRequest(ErrorMessages.INTERNAL_SERVER_ERROR);
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
        [ProducesResponseType(200, Type = typeof(FilteredStudent))]
        public async Task<ActionResult<FilteredStudent>> FilterStudents(int page = PAGE, int pageSize = PAGE_SIZE, string searchTerm = SEARCH_TERM)
        {
            if (page < 0)
            {
                throw new InvalidPageNumber(ErrorMessages.INVALID_PAGE_NUMBER);
            }

            if (pageSize <= 0)
            {
                throw new InvalidPageSize(ErrorMessages.INVALID_PAGE_SIZE);
            }

            var query = new FilterStudentQuery(page, pageSize, searchTerm);
            var (students, totalCount) = await _mediator.Send(query);

            if (!students.Any())
            {
                return NotFound(ErrorMessages.NO_STUDENTS_FOUND);
            }

            var studentDTOs = _mapper.Map<IEnumerable<StudentGetDTO>>(students);
            foreach (var student in studentDTOs)
            {
                student.Age = _studentService.CalculateAge(student.DateOfBirth);
            }
            return Ok(new FilteredStudent { Students = studentDTOs, TotalCount = totalCount });
        }
    }
}
