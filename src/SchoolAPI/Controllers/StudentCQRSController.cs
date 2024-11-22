using AutoMapper;
using CoreServices.CustomExceptions;
using CoreServices.StaticFiles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAPI.Business.Commands;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Queries;
using SchoolAPI.Business.Repository.Interfaces;
using SchoolAPI.Business.Services.Interfaces;
using SchoolAPI.DTO;
using SchoolAPI.StaticFiles;

namespace SchoolAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentCQRSController : ControllerBase
    {

        private readonly IStudentRepository _studentRepository;
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public StudentCQRSController(IStudentRepository studentRepository, IMapper mapper, IStudentService studentService, IMediator mediator)
        {
            _studentRepository = studentRepository;
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
        [ProducesResponseType(200, Type = typeof(StudentGetDTO))]
        public async Task<ActionResult<StudentGetDTO>> GetById(int id)
        {
            var student = await _mediator.Send(new GetStudentByIdQuery() { Id = id });
            if (student == null)
            {
                return NotFound(ErrorMessages.STUDENT_NOT_FOUND);
            }
            var studentDTO = _mapper.Map<StudentGetDTO>(student);
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

            var alreadyRegisteredStudent = await _studentRepository.GetByEmail(student.Email);

            if (alreadyRegisteredStudent != null)
            {
                throw new EmailAlreadyRegistered(student.Email + ErrorMessages.EMAIL_ALREADY_REGISTERED);
            }
            student.Age = _studentService.CalculateAge(student.DateOfBirth);

            var addedStudent = await _mediator.Send(new AddStudentCommand(
                student.FirstName,
                student.LastName,
                student.Email,
                student.Phone,
                student.DateOfBirth,
                student.Age
            ));
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
        [ProducesResponseType(200, Type = typeof(StudentGetDTO))]
        [Authorize(Roles = $"{AuthorizationRoles.ADMIN}")]
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
                existingStudent.Age = _studentService.CalculateAge((DateTime)studentUpdateDTO.DateOfBirth);
            }

            existingStudent.UpdatedAt = DateTime.Now;
            var dateOfBirth = studentUpdateDTO.DateOfBirth ?? existingStudent.DateOfBirth;
            var updatedStudent = await _mediator.Send(new UpdateStudentCommand(
                id,
                studentUpdateDTO.FirstName ?? existingStudent.FirstName,
                studentUpdateDTO.LastName ?? existingStudent.LastName,
                studentUpdateDTO.Email ?? existingStudent.Email,
                studentUpdateDTO.Phone ?? existingStudent.Phone,
                dateOfBirth
            ));
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
        [ProducesResponseType(200, Type = typeof(StudentGetDTO))]
        [Authorize(Roles = $"{AuthorizationRoles.ADMIN}")]
        public async Task<ActionResult<StudentGetDTO>> DeleteStudent(int id)
        {
            var student = await _studentRepository.GetById(id);
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
    }
}
