using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Services.Interfaces;
using SchoolAPI.DTO;
using SchoolAPI.Controllers;
using SchoolAPI.Business.Repository.Interfaces;
using SchoolAPI.StaticFiles;

public class StudentControllerTests
{
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly Mock<IStudentService> _studentServiceMock;
    private readonly IMapper _mapper;
    private readonly StudentController _controller;

    public StudentControllerTests()
    {
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _studentServiceMock = new Mock<IStudentService>();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Student, StudentGetDTO>();
            cfg.CreateMap<StudentPostDTO, Student>();
            cfg.CreateMap<StudentUpdateDTO, Student>();
        });
        _mapper = config.CreateMapper();
        _controller = new StudentController(_studentRepositoryMock.Object, _mapper, _studentServiceMock.Object);
    }

    [Fact]
    public async Task GetAllStudents_ShouldReturnOkResult_WithStudents_BestCase()
    {
        var students = new List<Student>
        {
            new Student
            {
                FirstName = "Aarav",
                LastName = "Sharma",
                Email = "aarav.sharma@example.com",
                Phone = "9876543210",
                DateOfBirth = new DateTime(2005, 5, 15),
                Age = 19,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            },
            new Student
            {
                FirstName = "Priya",
                LastName = "Verma",
                Email = "priya.verma@example.com",
                Phone = "9123456780",
                DateOfBirth = new DateTime(2004, 8, 20),
                Age = 20,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }
        };

        _studentRepositoryMock.Setup(repository => repository.GetAll()).ReturnsAsync(students);

        var result = await _controller.GetAllStudents();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<StudentGetDTO>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task GetAllStudents_ShouldThrowException_WhenExceptionOccurs()
    {
        _studentRepositoryMock.Setup(repository => repository.GetAll()).Throws(new Exception(ErrorMessages.STUDENT_NOT_FOUND));

        var exception = await Assert.ThrowsAsync<Exception>(async () => await _controller.GetAllStudents());

        Assert.Equal(ErrorMessages.STUDENT_NOT_FOUND, exception.Message);
    }


    [Fact]
    public async Task GetById_ShouldReturnOkResult_WhenStudentExists_BestCase()
    {
        var student = new Student
        {
            Id = 1,
            FirstName = "Aarav",
            LastName = "Sharma",
            Email = "aarav.sharma@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(2005, 5, 15),
            Age = 19,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        _studentRepositoryMock.Setup(repository => repository.GetById(1)).ReturnsAsync(student);

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<StudentGetDTO>(okResult.Value);
        Assert.Equal("Aarav", returnValue.FirstName);
    }

    [Fact]
    public async Task GetById_ShouldThrowException_WhenStudentDoesNotExist()
    {
        _studentRepositoryMock.Setup(repository => repository.GetById(999)).Throws(new Exception(ErrorMessages.STUDENT_NOT_FOUND));

        var exception = await Assert.ThrowsAsync<Exception>(async () => await _controller.GetById(999));

        Assert.Equal(ErrorMessages.STUDENT_NOT_FOUND, exception.Message);
    }

    [Fact]
    public async Task AddStudent_ShouldReturnOkResult_WhenValid_BestCase()
    {
        var studentPostDTO = new StudentPostDTO
        {
            FirstName = "Aarav",
            LastName = "Sharma",
            Email = "aarav.sharma@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(2005, 5, 15)
        };
        var student = new Student
        {
            Id = 1,
            FirstName = "Aarav",
            LastName = "Sharma",
            Email = "aarav.sharma@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(2005, 5, 15),
            Age = 19,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _studentRepositoryMock.Setup(repository => repository.Add(It.IsAny<Student>())).ReturnsAsync(student);

        var result = await _controller.AddStudent(studentPostDTO);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<StudentGetDTO>(okResult.Value);
        Assert.Equal("Aarav", returnValue.FirstName);
    }

    [Fact]
    public async Task AddStudent_ShouldReturnBadRequest_WhenValidationFails_WorstCase()
    {
        var invalidStudent = new StudentPostDTO
        { 
            FirstName = "",
            LastName = "", 
            Email = "invalid-email", 
            Phone = "9876543210",
            DateOfBirth = DateTime.Now.AddYears(1)
        };

        var student = new Student
        {
            FirstName = "",
            LastName = "", 
            Email = "invalid-email", 
            Phone = "9876543210",
            DateOfBirth = DateTime.Now,
            Age = 0,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var result = await _controller.AddStudent(invalidStudent);
        _studentRepositoryMock.Setup(repository => repository.Add(student)).ReturnsAsync(student);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = badRequestResult.Value as ValidationProblemDetails;

        Assert.NotNull(errors); 
        Assert.Contains("Email", errors.Errors.Keys); 
        Assert.Contains("FirstName", errors.Errors.Keys);
        Assert.Contains("LastName", errors.Errors.Keys); 
        Assert.Contains("DateOfBirth", errors.Errors.Keys);
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnOkResult_WhenStudentExists_BestCase()
    {
        var studentUpdateDTO = new StudentUpdateDTO
        {
            FirstName = "Aarav",
            LastName = "Sharma",
            Email = "aarav.sharma@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(2005, 5, 15)
        };
        var student = new Student
        {
            Id = 1,
            FirstName = "Aarav",
            LastName = "Sharma",
            Email = "aarav.sharma@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(2005, 5, 15),
            Age = 19,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _studentRepositoryMock.Setup(repository => repository.GetById(1)).ReturnsAsync(student);
        _studentRepositoryMock.Setup(repository => repository.Update(It.IsAny<Student>())).ReturnsAsync(student);

        var result = await _controller.UpdateStudent(1, studentUpdateDTO);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<StudentGetDTO>(okResult.Value);
        Assert.Equal("Aarav", returnValue.FirstName);
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnNotFound_WhenStudentDoesNotExist_WorstCase()
    {
        var studentUpdateDTO = new StudentUpdateDTO
        {
            FirstName = "Aarav",
            LastName = "Sharma",
            Email = "aarav.sharma@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(2005, 5, 15)
        };

        _studentRepositoryMock.Setup(repository => repository.GetById(999)).Throws(new Exception(ErrorMessages.STUDENT_NOT_FOUND));
        var exception = await Assert.ThrowsAsync<Exception>(async () => await _controller.UpdateStudent(999, studentUpdateDTO));
        Assert.Equal(ErrorMessages.STUDENT_NOT_FOUND, exception.Message);
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnBadRequest_WhenValidationFails_WorstCase()
    {
        // Arrange
        var invalidStudentUpdate = new StudentUpdateDTO
        {
            FirstName = "",
            LastName = "",
            Email = "invalid-email",
            Phone = "9876543210",
            DateOfBirth = DateTime.Now.AddYears(1)
        };

        var student = new Student
        {
            Id = 1,
            FirstName = "Aarav",
            LastName = "Sharma",
            Email = "aarav.sharma@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(2005, 5, 15),
            Age = 19,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _studentRepositoryMock.Setup(repository => repository.GetById(1)).ReturnsAsync(student);
        // Act
        var result = await _controller.UpdateStudent(1, invalidStudentUpdate);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = badRequestResult.Value as ValidationProblemDetails;

        Assert.NotNull(errors);
        Assert.Contains("Email", errors.Errors.Keys);
        Assert.Contains("FirstName", errors.Errors.Keys);
        Assert.Contains("LastName", errors.Errors.Keys);
        Assert.Contains("DateOfBirth", errors.Errors.Keys);
    }

    [Fact]
    public async Task DeleteStudent_ShouldReturnOkResult_WhenStudentExists_BestCase()
    {
        var student = new Student { Id = 1, FirstName = "Aarav" };
        _studentRepositoryMock.Setup(repository => repository.GetById(1)).ReturnsAsync(student);
        _studentRepositoryMock.Setup(repository => repository.Delete(1)).ReturnsAsync(true);

        var result = await _controller.DeleteStudent(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(student.FirstName, ((StudentGetDTO)okResult.Value).FirstName);
    }

    [Fact]
    public async Task DeleteStudent_ShouldThrowException_WhenStudentDoesNotExist()
    {
        _studentRepositoryMock.Setup(repository => repository.Delete(999)).ThrowsAsync(new Exception(ErrorMessages.STUDENT_NOT_FOUND));
        var exception = await Assert.ThrowsAsync<Exception>(async () => await _controller.DeleteStudent(999));
        Assert.Equal(ErrorMessages.STUDENT_NOT_FOUND, exception.Message);
    }


    [Fact]
    public async Task FilterStudents_ShouldReturnOkResult_WithFilteredStudents_BestCase()
    {
        var students = new List<Student>
        {
            new Student
            {
                FirstName = "Aarav",
                LastName = "Sharma",
                Email = "aarav.sharma@example.com",
                Phone = "9876543210",
                DateOfBirth = new DateTime(2005, 5, 15),
                Age = 19,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            },
            new Student
            {
                FirstName = "Divya",
                LastName = "Verma",
                Email = "divya.verma@example.com",
                Phone = "9876543310",
                DateOfBirth = new DateTime(2004, 5, 15),
                Age = 20,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }
        };

        var filteredStudents = new List<Student>
        {
            students[0]
        };
        var totalCount = 1;

        _studentRepositoryMock
            .Setup(repository => repository.FilterStudents(1, 10, "Aarav"))
            .ReturnsAsync((filteredStudents, totalCount));

        var result = await _controller.FilterStudents(1, 10, "Aarav");
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<FilteredStudent>(okResult.Value);
        var studentDTOs = returnValue.Students.ToList();

        Assert.Single(studentDTOs);
        Assert.Equal(1, returnValue.TotalCount);
        Assert.Equal("Aarav", studentDTOs[0].FirstName);
        Assert.Equal("Sharma", studentDTOs[0].LastName);
    }

    [Fact]
    public async Task FilterStudents_ShouldReturnOkResult_Empty_WhenNoMatches_WorstCase()
    {
        var filteredStudents = new List<Student>();
        _studentRepositoryMock.Setup(repository => repository.FilterStudents(1, 10, "NonExistent"))
            .ReturnsAsync((filteredStudents, 0));

        var result = await _controller.FilterStudents(1, 10, "NonExistent");
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<FilteredStudent>(okResult.Value);
        var studentDTOs = Assert.IsAssignableFrom<IEnumerable<StudentGetDTO>>(returnValue.Students);
        Assert.Empty(studentDTOs);
        Assert.Equal(0, returnValue.TotalCount);
    }
}
