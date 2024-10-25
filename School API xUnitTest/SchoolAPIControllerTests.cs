using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Services.Interfaces;
using SchoolAPI.DTO;
using SchoolAPI.Controllers;
using SchoolAPI.Business;

public class StudentControllerTests
{
    private readonly Mock<IStudentService> _studentServiceMock;
    private readonly IMapper _mapper;
    private readonly StudentController _controller;

    public StudentControllerTests()
    {
        _studentServiceMock = new Mock<IStudentService>();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Student, StudentGetDTO>();
            cfg.CreateMap<StudentPostDTO, Student>();
        });
        _mapper = config.CreateMapper();
        _controller = new StudentController(_studentServiceMock.Object, _mapper);
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

        _studentServiceMock.Setup(service => service.GetAll()).ReturnsAsync(students);

        var result = await _controller.GetAllStudents();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<StudentGetDTO>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task GetAllStudents_ShouldReturnInternalServerError_WhenExceptionOccurs_WorstCase()
    {
        _studentServiceMock.Setup(service => service.GetAll()).ThrowsAsync(new Exception("Database error"));

        var result = await _controller.GetAllStudents();

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Contains("Internal server error", statusCodeResult.Value.ToString());
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
        _studentServiceMock.Setup(service => service.GetById(1)).ReturnsAsync(student);

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<StudentGetDTO>(okResult.Value);
        Assert.Equal("Aarav", returnValue.FirstName);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenStudentDoesNotExist_WorstCase()
    {
        _studentServiceMock.Setup(service => service.GetById(999)).ReturnsAsync((Student)null);

        var result = await _controller.GetById(999);

        Assert.IsType<NotFoundObjectResult>(result);
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

        _studentServiceMock.Setup(service => service.Add(It.IsAny<Student>())).ReturnsAsync(student);

        var result = await _controller.AddStudent(studentPostDTO);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<StudentGetDTO>(okResult.Value);
        Assert.Equal("Aarav", returnValue.FirstName);
    }

    [Fact]
    public async Task AddStudent_ShouldReturnBadRequest_WhenValidationFails_WorstCase()
    {
        var studentPostDTO = new StudentPostDTO
        {
            FirstName = "", // Invalid input
            LastName = "Sharma",
            Email = "aarav.sharma@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(2005, 5, 15)
        };

        var result = await _controller.AddStudent(studentPostDTO);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Property FirstName failed validation", badRequestResult.Value.ToString());
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnOkResult_WhenStudentExists_BestCase()
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

        _studentServiceMock.Setup(service => service.GetById(1)).ReturnsAsync(student);
        _studentServiceMock.Setup(service => service.Update(1, It.IsAny<Student>())).ReturnsAsync(student);

        var result = await _controller.UpdateStudent(1, studentPostDTO);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<StudentGetDTO>(okResult.Value);
        Assert.Equal("Aarav", returnValue.FirstName);
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnNotFound_WhenStudentDoesNotExist_WorstCase()
    {
        var studentPostDTO = new StudentPostDTO
        {
            FirstName = "Aarav",
            LastName = "Sharma",
            Email = "aarav.sharma@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(2005, 5, 15)
        };

        _studentServiceMock.Setup(service => service.GetById(999)).ReturnsAsync((Student)null);
        var result = await _controller.UpdateStudent(999, studentPostDTO);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteStudent_ShouldReturnOkResult_WhenStudentExists_BestCase()
    {
        _studentServiceMock.Setup(service => service.Delete(1)).ReturnsAsync("Student deleted successfully");

        var result = await _controller.DeleteStudent(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Student deleted successfully", okResult.Value);
    }

    [Fact]
    public async Task DeleteStudent_ShouldReturnNotFound_WhenStudentDoesNotExist_WorstCase()
    {
        _studentServiceMock.Setup(service => service.Delete(999)).ReturnsAsync("Student not found");

        var result = await _controller.DeleteStudent(999);

        Assert.IsType<NotFoundObjectResult>(result);
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

        FilteredStudent filteredStudent = new FilteredStudent() {
            Students = new List<Student> {students[0]},
            TotalCount = 1
        };

        _studentServiceMock.Setup(service => service.FilterStudents(1, 10, "Aarav"))
            .ReturnsAsync(filteredStudent);

        var result = await _controller.FilterStudents(1, 10, "Aarav");
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<(IEnumerable<StudentGetDTO>,int)>(okResult.Value);
        var studentDTOs = Assert.IsAssignableFrom<IEnumerable<StudentGetDTO>>(returnValue.Item1);
        Assert.Single(studentDTOs);
    }

    [Fact]
    public async Task FilterStudents_ShouldReturnOkResult_Empty_WhenNoMatches_WorstCase()
    {
        _studentServiceMock.Setup(service => service.FilterStudents(1, 10, "NonExistent"))
            .ReturnsAsync(new FilteredStudent {Students = [], TotalCount = 0});

        var result = await _controller.FilterStudents(1, 10, "NonExistent");
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<(IEnumerable<StudentGetDTO>,int)>(okResult.Value);
        var studentDTOs = Assert.IsAssignableFrom<IEnumerable<StudentGetDTO>>(returnValue.Item1);
        Assert.Empty(studentDTOs);
        Assert.Equal(0, returnValue.Item2);
    }
}
