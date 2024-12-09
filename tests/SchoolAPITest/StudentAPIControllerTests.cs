using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Services.Interfaces;
using SchoolAPI.DTO;
using SchoolAPI.Controllers;
using SchoolAPI.Business.Repository.Interfaces;
using Bogus;
using SchoolAPI.StaticFiles;
using Plain.RabbitMQ;
using SchoolAPITest.Helpers;

public class StudentControllerTests
{
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly Mock<IStudentService> _studentServiceMock;
    private readonly Mock<IPublisher> _publisher;
    private readonly IMapper _mapper;
    private readonly StudentController _controller;
    private readonly Faker<Student> _studentFaker;

    public StudentControllerTests()
    {
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _studentServiceMock = new Mock<IStudentService>();
        _publisher = new Mock<IPublisher>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Student, StudentGetDTO>().ReverseMap();
            cfg.CreateMap<StudentPostDTO, Student>().ReverseMap();
            cfg.CreateMap<StudentUpdateDTO, Student>().ReverseMap();
        });
        _mapper = config.CreateMapper();
        _controller = new StudentController(_studentRepositoryMock.Object, _mapper, _studentServiceMock.Object, _publisher.Object);

        _studentFaker = StudentFaker.CreateFaker();
    }

    [Fact]
    public async Task GetAllStudents_ShouldReturnOkResult_WithStudents_BestCase()
    {
        var students = _studentFaker.Generate(2);
        _studentRepositoryMock.Setup(repo => repo.GetAll()).ReturnsAsync(students);

        var result = await _controller.GetAllStudents();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<StudentGetDTO>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());

        foreach (var student in students)
        {
            var studentDTO = returnValue.First(s => s.FirstName == student.FirstName && s.LastName == student.LastName);
            Assert.Equal(student.FirstName, studentDTO.FirstName);
            Assert.Equal(student.LastName, studentDTO.LastName);
            Assert.Equal(student.Email, studentDTO.Email);
            Assert.Equal(student.Phone, studentDTO.Phone);
            Assert.Equal(student.DateOfBirth, studentDTO.DateOfBirth);
        }
    }

    [Fact]
    public async Task GetById_ShouldReturnOkResult_WhenStudentExists_BestCase()
    {
        var student = _studentFaker.Generate();
        _studentRepositoryMock.Setup(repo => repo.GetById(student.Id)).ReturnsAsync(student);

        var result = await _controller.GetById(student.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<StudentGetDTO>(okResult.Value);
        Assert.Equal(student.FirstName, returnValue.FirstName);
        Assert.Equal(student.LastName, returnValue.LastName);
        Assert.Equal(student.Email, returnValue.Email);
        Assert.Equal(student.Phone, returnValue.Phone);
        Assert.Equal(student.DateOfBirth, returnValue.DateOfBirth);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenStudentDoesNotExist()
    {
        _studentRepositoryMock.Setup(repo => repo.GetById(999)).ReturnsAsync((Student)null);

        var result = await _controller.GetById(999);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(ErrorMessages.STUDENT_NOT_FOUND, notFoundResult.Value);
    }

    [Fact]
    public async Task AddStudent_ShouldReturnOkResult_WhenValid_BestCase()
    {
        var student = _studentFaker.Generate();
        var studentPostDTO = _mapper.Map<StudentPostDTO>(student);

        _studentRepositoryMock.Setup(repo => repo.Add(It.IsAny<Student>())).ReturnsAsync(student);
        _studentRepositoryMock.Setup(repo => repo.GetByEmail(studentPostDTO.Email)).ReturnsAsync((Student)null); // Ensure no email conflict

        var result = await _controller.AddStudent(studentPostDTO);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<StudentGetDTO>(okResult.Value);
        Assert.Equal(student.FirstName, returnValue.FirstName);
        Assert.Equal(student.LastName, returnValue.LastName);
        Assert.Equal(student.Email, returnValue.Email);
        Assert.Equal(student.Phone, returnValue.Phone);
        Assert.Equal(student.DateOfBirth, returnValue.DateOfBirth);
    }


    [Fact]
    public async Task UpdateStudent_ShouldReturnOkResult_WhenStudentExists_BestCase()
    {
        var student = _studentFaker.Generate();
        var studentUpdateDTO = _mapper.Map<StudentUpdateDTO>(student);

        _studentRepositoryMock.Setup(repo => repo.GetById(student.Id)).ReturnsAsync(student);
        _studentRepositoryMock.Setup(repo => repo.Update(It.IsAny<Student>())).ReturnsAsync(student);

        var result = await _controller.UpdateStudent(student.Id, studentUpdateDTO);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<StudentGetDTO>(okResult.Value);
        Assert.Equal(student.FirstName, returnValue.FirstName);
        Assert.Equal(student.LastName, returnValue.LastName);
        Assert.Equal(student.Email, returnValue.Email);
        Assert.Equal(student.Phone, returnValue.Phone);
        Assert.Equal(student.DateOfBirth, returnValue.DateOfBirth);
    }


    [Fact]
    public async Task UpdateStudent_ShouldReturnNotFound_WhenStudentDoesNotExist_WorstCase()
    {
        var student = _studentFaker.Generate();
        var studentUpdateDTO = _mapper.Map<StudentUpdateDTO>(student);
        _studentRepositoryMock.Setup(repo => repo.GetById(999)).ReturnsAsync((Student)null);

        var result = await _controller.UpdateStudent(999, studentUpdateDTO);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(ErrorMessages.STUDENT_NOT_FOUND, notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteStudent_ShouldReturnOkResult_WhenStudentExists_BestCase()
    {
        // Arrange
        var student = _studentFaker.Generate();
        _studentRepositoryMock.Setup(repo => repo.GetById(student.Id)).ReturnsAsync(student);
        _studentRepositoryMock.Setup(repo => repo.Delete(student.Id)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteStudent(student.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var studentDTO = Assert.IsType<StudentGetDTO>(okResult.Value);

        Assert.Equal(student.FirstName, studentDTO.FirstName);
        Assert.Equal(student.LastName, studentDTO.LastName);
    }

    [Fact]
    public async Task DeleteStudent_ShouldReturnNotFound_WhenStudentDoesNotExist()
    {
        _studentRepositoryMock.Setup(repo => repo.GetById(999)).ReturnsAsync((Student)null);
        var result = await _controller.DeleteStudent(999);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(ErrorMessages.STUDENT_NOT_FOUND, notFoundResult.Value);
    }

    [Fact]
    public async Task FilterStudents_ShouldReturnOkResult_WithFilteredStudents_BestCase()
    {
        var student1 = _studentFaker.Generate();
        var student2 = _studentFaker.Generate();
        // student1.Id = 1;
        // student2.Id = 2;

        var filteredStudents = new List<Student> { student1 };
        var totalCount = 1;

        _studentRepositoryMock
            .Setup(repo => repo.FilterStudents(1, 10, student1.FirstName))
            .ReturnsAsync((filteredStudents, totalCount));

        var result = await _controller.FilterStudents(1, 10, student1.FirstName);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<FilteredStudent>(okResult.Value);
        var studentDTOs = returnValue.Students.ToList();

        Assert.Single(studentDTOs);
        Assert.Equal(1, returnValue.TotalCount);
        Assert.Equal(student1.FirstName, studentDTOs[0].FirstName);
        Assert.Equal(student1.LastName, studentDTOs[0].LastName);
        Assert.Equal(student1.Email, studentDTOs[0].Email);
        Assert.Equal(student1.Phone, studentDTOs[0].Phone);
        Assert.Equal(student1.DateOfBirth, studentDTOs[0].DateOfBirth);
    }
}
