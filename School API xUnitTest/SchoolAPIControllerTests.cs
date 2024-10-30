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


public class StudentControllerTests
{
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly Mock<IStudentService> _studentServiceMock;
    private readonly IMapper _mapper;
    private readonly StudentController _controller;
    private readonly Faker<Student> _studentFaker;
    private readonly Faker<StudentPostDTO> _studentPostFaker;
    private readonly Faker<StudentUpdateDTO> _studentUpdateFaker;

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


        _studentFaker = new Faker<Student>()
            .RuleFor(s => s.FirstName, f => f.Name.FirstName())
            .RuleFor(s => s.LastName, f => f.Name.LastName())
            .RuleFor(s => s.Email, f => f.Internet.Email())
            .RuleFor(s => s.Phone, f =>
            {
                var firstDigit = f.Random.Int(7, 9);
                var remainingDigits = f.Random.Number(10000000, 99999999);
                return $"{firstDigit}{remainingDigits}";
            })
            .RuleFor(s => s.DateOfBirth, f => f.Date.Past(20, null))
            .RuleFor(s => s.Age, (f, s) => DateTime.Now.Year - s.DateOfBirth.Year)
            .RuleFor(s => s.CreatedAt, f => DateTime.Now)
            .RuleFor(s => s.UpdatedAt, f => DateTime.Now)
            .RuleFor(s => s.isActive, true);


        _studentPostFaker = new Faker<StudentPostDTO>()
            .RuleFor(s => s.FirstName, f => f.Name.FirstName())
            .RuleFor(s => s.LastName, f => f.Name.LastName())
            .RuleFor(s => s.Email, f => f.Internet.Email())
            .RuleFor(s => s.Phone, f =>
            {
                var firstDigit = f.Random.Int(7, 9);
                var remainingDigits = f.Random.Number(10000000, 99999999);
                return $"{firstDigit}{remainingDigits}";
            })
            .RuleFor(s => s.DateOfBirth, f => f.Date.Past(20, null));

        _studentUpdateFaker = new Faker<StudentUpdateDTO>()
            .RuleFor(s => s.FirstName, f => f.Name.FirstName())
            .RuleFor(s => s.LastName, f => f.Name.LastName())
            .RuleFor(s => s.Email, f => f.Internet.Email())
            .RuleFor(s => s.Phone, f =>
            {
                var firstDigit = f.Random.Int(7, 9);
                var remainingDigits = f.Random.Number(10000000, 99999999);
                return $"{firstDigit}{remainingDigits}";
            })
            .RuleFor(s => s.DateOfBirth, f => f.Date.Past(20, null));
    }

    [Fact]
    public async Task GetAllStudents_ShouldReturnOkResult_WithStudents_BestCase()
    {
        var students = _studentFaker.Generate(2);

        _studentRepositoryMock.Setup(repository => repository.GetAll()).ReturnsAsync(students);

        var result = await _controller.GetAllStudents();

        var okResult = Assert.IsType<OkObjectResult>(result);
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
            Assert.Equal(student.Age, studentDTO.Age);
        }
    }

    [Fact]
    public async Task GetById_ShouldReturnOkResult_WhenStudentExists_BestCase()
    {
        var student = _studentFaker.Generate();
        student.Id = 1;
        _studentRepositoryMock.Setup(repository => repository.GetById(1)).ReturnsAsync(student);

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<StudentGetDTO>(okResult.Value);
        Assert.Equal(student.FirstName, returnValue.FirstName);
        Assert.Equal(student.LastName, returnValue.LastName);
        Assert.Equal(student.Email, returnValue.Email);
        Assert.Equal(student.Phone, returnValue.Phone);
        Assert.Equal(student.DateOfBirth, returnValue.DateOfBirth);
        Assert.Equal(student.Age, returnValue.Age);
    }

    [Fact]
    public async Task GetById_ShouldThrowException_WhenStudentDoesNotExist()
    {
        _studentRepositoryMock.Setup(repository => repository.GetById(999)).ReturnsAsync((Student)null);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _controller.GetById(999));
        Assert.Equal(ErrorMessages.STUDENT_NOT_FOUND, exception.Message);
    }


    [Fact]
    public async Task AddStudent_ShouldReturnOkResult_WhenValid_BestCase()
    {
        var studentPostDTO = _studentPostFaker.Generate();

        var student = _studentFaker.Generate();
        student.Id = 1;

        _studentRepositoryMock.Setup(repository => repository.Add(It.IsAny<Student>())).ReturnsAsync(student);

        var result = await _controller.AddStudent(studentPostDTO);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<StudentGetDTO>(okResult.Value);
        Assert.Equal(student.FirstName, returnValue.FirstName);
        Assert.Equal(student.LastName, returnValue.LastName);
        Assert.Equal(student.Email, returnValue.Email);
        Assert.Equal(student.Phone, returnValue.Phone);
        Assert.Equal(student.DateOfBirth, returnValue.DateOfBirth);
        Assert.Equal(student.Age, returnValue.Age);
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
        var result = await _controller.AddStudent(invalidStudent);
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
        var studentUpdateDTO = _studentUpdateFaker.Generate();

        var student = _studentFaker.Generate();
        student.Id = 1;

        _studentRepositoryMock.Setup(repository => repository.GetById(1)).ReturnsAsync(student);
        _studentRepositoryMock.Setup(repository => repository.Update(It.IsAny<Student>())).ReturnsAsync(student);

        var result = await _controller.UpdateStudent(1, studentUpdateDTO);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<StudentGetDTO>(okResult.Value);
        Assert.Equal(student.FirstName, returnValue.FirstName);
        Assert.Equal(student.LastName, returnValue.LastName);
        Assert.Equal(student.Email, returnValue.Email);
        Assert.Equal(student.Phone, returnValue.Phone);
        Assert.Equal(student.DateOfBirth, returnValue.DateOfBirth);
        Assert.Equal(student.Age, returnValue.Age);
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnNotFound_WhenStudentDoesNotExist_WorstCase()
    {
        var studentUpdateDTO = _studentUpdateFaker.Generate();

        _studentRepositoryMock.Setup(repository => repository.GetById(999)).Throws(new Exception(ErrorMessages.STUDENT_NOT_FOUND));
        var exception = await Assert.ThrowsAsync<Exception>(async () => await _controller.UpdateStudent(999, studentUpdateDTO));
        Assert.Equal(ErrorMessages.STUDENT_NOT_FOUND, exception.Message);
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnBadRequest_WhenValidationFails_WorstCase()
    {
        var invalidStudentUpdate = new StudentUpdateDTO
        {
            FirstName = "",
            LastName = "",
            Email = "invalid-email",
            Phone = "9876543210",
            DateOfBirth = DateTime.Now.AddYears(1)
        };

        var student = _studentFaker.Generate();
        student.Id = 1;

        _studentRepositoryMock.Setup(repository => repository.GetById(1)).ReturnsAsync(student);

        var result = await _controller.UpdateStudent(1, invalidStudentUpdate);

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
        var student = new Student { Id = 1, FirstName = "Aarav", LastName = "Sharma" };
        _studentRepositoryMock.Setup(repository => repository.GetById(1)).ReturnsAsync(student);
        _studentRepositoryMock.Setup(repository => repository.Delete(1)).ReturnsAsync(true);

        var result = await _controller.DeleteStudent(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(student.FirstName, ((StudentGetDTO)okResult.Value).FirstName);
        Assert.Equal(student.LastName, ((StudentGetDTO)okResult.Value).LastName);
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
        var student1 = _studentFaker.Generate();
        var student2 = _studentFaker.Generate();
        student1.Id = 1;
        student2.Id = 2;

        var filteredStudents = new List<Student> { student1 };
        var totalCount = 1;

        _studentRepositoryMock
            .Setup(repository => repository.FilterStudents(1, 10, student1.FirstName))
            .ReturnsAsync((filteredStudents, totalCount));

        var result = await _controller.FilterStudents(1, 10, student1.FirstName);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<FilteredStudent>(okResult.Value);
        var studentDTOs = returnValue.Students.ToList();

        Assert.Single(studentDTOs);
        Assert.Equal(1, returnValue.TotalCount);
        Assert.Equal(student1.FirstName, studentDTOs[0].FirstName);
        Assert.Equal(student1.LastName, studentDTOs[0].LastName);
        Assert.Equal(student1.Email, studentDTOs[0].Email);
        Assert.Equal(student1.Phone, studentDTOs[0].Phone);
        Assert.Equal(student1.DateOfBirth, studentDTOs[0].DateOfBirth);
        Assert.Equal(student1.Age, studentDTOs[0].Age);
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
