using AutoMapper;
using Bogus;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Services.Interfaces;
using SchoolAPI.Commands;
using SchoolAPI.Controllers;
using SchoolAPI.DTO;
using SchoolAPI.Queries;
using SchoolAPI.StaticFiles;
using SchoolAPITest.Helpers;

namespace SchoolAPITest
{
    public class StudentAPICQRSControllerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IStudentService> _mockStudentService;
        private readonly Mock<IMediator> _mockMediator;
        private readonly StudentCQRSController _controller;
        private readonly Faker<Student> _studentFaker;

        public StudentAPICQRSControllerTest()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Student, StudentGetDTO>().ReverseMap();
                cfg.CreateMap<StudentPostDTO, Student>().ReverseMap();
                cfg.CreateMap<StudentUpdateDTO, Student>().ReverseMap();
            });

            _mapper = config.CreateMapper();
            _mockStudentService = new Mock<IStudentService>();
            _mockMediator = new Mock<IMediator>();
            _controller = new StudentCQRSController(
                _mapper,
                _mockStudentService.Object,
                _mockMediator.Object
            );

            _studentFaker = StudentFaker.CreateFaker();
        }

        [Fact]
        public async Task GetAllStudents_ShouldReturnOk_WhenStudentsExist()
        {
            // Arrange
            var students = _studentFaker.Generate(2);
            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllStudentQuery>(), default)).ReturnsAsync(students);

            // Act
            var result = await _controller.GetAllStudents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedStudents = Assert.IsAssignableFrom<IEnumerable<StudentGetDTO>>(okResult.Value);
            Assert.Equal(2, returnedStudents.Count());

            foreach (var student in students)
            {
                var studentDTO = returnedStudents.First(s => s.FirstName == student.FirstName && s.LastName == student.LastName);
                Assert.Equal(student.FirstName, studentDTO.FirstName);
                Assert.Equal(student.LastName, studentDTO.LastName);
                Assert.Equal(student.Email, studentDTO.Email);
                Assert.Equal(student.Phone, studentDTO.Phone);
                Assert.Equal(student.DateOfBirth, studentDTO.DateOfBirth);
            }
        }

        [Fact]
        public async Task GetAllStudents_ShouldReturnNotFound_WhenNoStudentsExist()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetAllStudentQuery>(), default)).ReturnsAsync((IEnumerable<Student>)null);
            var result = await _controller.GetAllStudents();
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task AddStudent_ShouldReturnOk_WhenStudentIsAdded()
        {
            var student = _studentFaker.Generate();
            var studentPostDTO = _mapper.Map<StudentPostDTO>(student);

            _mockMediator.Setup(m => m.Send(It.IsAny<AddStudentCommand>(), default)).ReturnsAsync(student);

            var result = await _controller.AddStudent(studentPostDTO);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedStudent = Assert.IsType<StudentGetDTO>(okResult.Value);
            Assert.Equal(student.FirstName, returnedStudent.FirstName);
            Assert.Equal(student.LastName, returnedStudent.LastName);
            Assert.Equal(student.Email, returnedStudent.Email);
            Assert.Equal(student.Phone, returnedStudent.Phone);
            Assert.Equal(student.DateOfBirth, returnedStudent.DateOfBirth);
        }

        [Fact]
        public async Task GetStudentById_ShouldReturnOk_WhenStudentExists()
        {
            var student = _studentFaker.Generate();
            _mockMediator.Setup(m => m.Send(It.IsAny<GetStudentByIdQuery>(), default)).ReturnsAsync(student);

            var result = await _controller.GetById(student.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedStudent = Assert.IsType<StudentGetDTO>(okResult.Value);
            Assert.Equal(student.FirstName, returnedStudent.FirstName);
            Assert.Equal(student.LastName, returnedStudent.LastName);
            Assert.Equal(student.Email, returnedStudent.Email);
            Assert.Equal(student.Phone, returnedStudent.Phone);
            Assert.Equal(student.DateOfBirth, returnedStudent.DateOfBirth);
        }

        [Fact]
        public async Task GetStudentById_ShouldReturnNotFound_WhenStudentDoesNotExist()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetStudentByIdQuery>(), default)).ReturnsAsync((Student)null);

            var result = await _controller.GetById(1);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateStudent_ShouldReturnOk_WhenStudentIsUpdated()
        {
            var existingStudent = _studentFaker.Generate();
            var updatedStudent = _studentFaker.Generate();
            updatedStudent.FirstName = "UpdatedFirstName";
            updatedStudent.LastName = "UpdatedLastName";

            var studentUpdateDTO = _mapper.Map<StudentUpdateDTO>(updatedStudent);

            _mockMediator.Setup(m => m.Send(It.IsAny<GetStudentByIdQuery>(), default)).ReturnsAsync(existingStudent);
            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateStudentCommand>(), default)).ReturnsAsync(updatedStudent);

            var result = await _controller.UpdateStudent(existingStudent.Id, studentUpdateDTO);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedStudent = Assert.IsType<StudentGetDTO>(okResult.Value);

            Assert.Equal(updatedStudent.FirstName, returnedStudent.FirstName);
            Assert.Equal(updatedStudent.LastName, returnedStudent.LastName);
            Assert.Equal(updatedStudent.Email, returnedStudent.Email);
            Assert.Equal(updatedStudent.Phone, returnedStudent.Phone);
            Assert.Equal(updatedStudent.DateOfBirth, returnedStudent.DateOfBirth);
        }

        [Fact]
        public async Task UpdateStudent_ShouldReturnNotFound_WhenStudentDoesNotExist()
        {
            var studentUpdateDTO = _mapper.Map<StudentUpdateDTO>(_studentFaker.Generate());
            _mockMediator.Setup(m => m.Send(It.IsAny<GetStudentByIdQuery>(), default)).ReturnsAsync((Student)null);

            var result = await _controller.UpdateStudent(1, studentUpdateDTO);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteStudent_ShouldReturnDeletedStudent_WhenStudentIsDeleted()
        {
            var student = _studentFaker.Generate();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetStudentByIdQuery>(), default)).ReturnsAsync(student);
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteStudentCommand>(), default)).ReturnsAsync(true);

            var result = await _controller.DeleteStudent(student.Id);

            Assert.IsType<ActionResult<StudentGetDTO>>(result);
        }

        [Fact]
        public async Task DeleteStudent_ShouldReturnNotFound_WhenStudentDoesNotExist()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetStudentByIdQuery>(), default)).ReturnsAsync((Student)null);

            var result = await _controller.DeleteStudent(1);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(ErrorMessages.STUDENT_NOT_FOUND, notFoundResult.Value);
        }
    }
}
