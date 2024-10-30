using Microsoft.EntityFrameworkCore;
using SchoolAPI.Business.Data;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Repository;
using SchoolAPI.Business.Repository.Interfaces;

public class StudentRepositoryTests : IAsyncLifetime
{
    private SchoolAPIDbContext _context;
    private IStudentRepository _repository;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<SchoolAPIDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new SchoolAPIDbContext(options);
        _repository = new StudentRepository(_context);
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task Add_ShouldAddStudent_BestCase()
    {
        var student = new Student
        {
            FirstName = "Aarav",
            LastName = "Sharma",
            Email = "aarav.sharma@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(2005, 5, 15),
            Age = 19,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var result = await _repository.Add(student);

        Assert.NotNull(result);
        Assert.Equal("Aarav", result.FirstName);
        Assert.Single(await _context.Students.ToListAsync());
    }

    [Fact]
    public async Task Add_ShouldThrowException_WhenStudentIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.Add(null));
    }

    [Fact]
    public async Task GetById_ShouldReturnStudent_WhenExists_BestCase()
    {
        var student = new Student
        {
            FirstName = "Aarav",
            LastName = "Sharma",
            Email = "aarav.sharma@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(2005, 5, 15),
            Age = 19,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        await _repository.Add(student);

        var result = await _repository.GetById(1);

        Assert.NotNull(result);
        Assert.Equal("Aarav", result.FirstName);
    }

    [Fact]
    public async Task GetById_ShouldReturnNull_WhenNotExists_WorstCase()
    {
        var result = await _repository.GetById(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_ShouldMarkStudentAsInactive_WhenExists_BestCase()
    {
        var student = new Student
        {
            Id = 2,
            FirstName = "Aarav",
            LastName = "Sharma",
            Email = "aarav.sharma@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(2005, 5, 15),
            Age = 19,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        await _repository.Add(student);

        var result = await _repository.Delete(2);

        Assert.True(result);
        var deletedStudent = await _repository.GetById(2);
        Assert.Null(deletedStudent);
    }

    [Fact]
    public async Task Delete_ShouldReturnFalse_WhenNotExists_WorstCase()
    {
        var result = await _repository.Delete(999);
        Assert.False(result);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllActiveStudents_BestCase()
    {
        var student1 = new Student
        {
            FirstName = "Aarav",
            LastName = "Sharma",
            Email = "aarav.sharma@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(2005, 5, 15),
            Age = 19,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        var student2 = new Student
        {
            FirstName = "Priya",
            LastName = "Verma",
            Email = "priya.verma@example.com",
            Phone = "9123456780",
            DateOfBirth = new DateTime(2004, 8, 20),
            Age = 20,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        await _repository.Add(student1);
        await _repository.Add(student2);

        var result = await _repository.GetAll();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmpty_WhenNoActiveStudents_WorstCase()
    {
        var student1 = new Student
        {
            FirstName = "Aarav",
            LastName = "Sharma",
            Email = "aarav.sharma@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(2005, 5, 15),
            Age = 19,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            isActive = false
        };
        await _repository.Add(student1);

        var result = await _repository.GetAll();

        Assert.Empty(result);
    }

    [Fact]
    public async Task FilterStudents_ShouldReturnFilteredStudents_BestCase()
    {
        var student1 = new Student
        {
            FirstName = "Aarav",
            LastName = "Sharma",
            Email = "aarav.sharma@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(2005, 5, 15),
            Age = 19,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        var student2 = new Student
        {
            FirstName = "Priya",
            LastName = "Verma",
            Email = "priya.verma@example.com",
            Phone = "9123456780",
            DateOfBirth = new DateTime(2004, 8, 20),
            Age = 20,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        await _repository.Add(student1);
        await _repository.Add(student2);

        var (students, totalCount) = await _repository.FilterStudents(1, 10, "Aarav");

        Assert.Single(students);
        Assert.Equal(1, totalCount);
    }

    [Fact]
    public async Task FilterStudents_ShouldReturnEmpty_WhenNoMatches_WorstCase()
    {
        var student1 = new Student
        {
            FirstName = "Aarav",
            LastName = "Sharma",
            Email = "aarav.sharma@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(2005, 5, 15),
            Age = 19,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        await _repository.Add(student1);

        var (students, totalCount) = await _repository.FilterStudents(1, 10, "NonExistent");

        Assert.Empty(students);
        Assert.Equal(0, totalCount);
    }
}
