using Bogus; // Add this using directive
using Microsoft.EntityFrameworkCore;
using SchoolAPI.Business.Data;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Repository;
using SchoolAPI.Business.Repository.Interfaces;

public class StudentRepositoryTests : IAsyncLifetime
{
    private SchoolAPIDbContext _context;
    private IStudentRepository _repository;
    private readonly Faker<Student> _studentFaker = new Faker<Student>()
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
        var student = _studentFaker.Generate();

        var result = await _repository.Add(student);

        Assert.NotNull(result);
        Assert.Equal(student.FirstName, result.FirstName);
        Assert.Equal(student.LastName, result.LastName);
        Assert.Equal(student.Email, result.Email);
        Assert.Equal(student.Phone, result.Phone);
        Assert.Equal(student.DateOfBirth, result.DateOfBirth);
        Assert.Equal(student.Age, result.Age);
        Assert.Equal(student.CreatedAt, result.CreatedAt);
        Assert.Equal(student.UpdatedAt, result.UpdatedAt);
        Assert.Equal(student.isActive, result.isActive);
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
        var student = _studentFaker.Generate();
        await _repository.Add(student);

        var result = await _repository.GetById(student.Id);

        Assert.NotNull(result);
        Assert.Equal(student.FirstName, result.FirstName);
        Assert.Equal(student.LastName, result.LastName);
        Assert.Equal(student.Email, result.Email);
        Assert.Equal(student.Phone, result.Phone);
        Assert.Equal(student.DateOfBirth, result.DateOfBirth);
        Assert.Equal(student.Age, result.Age);
        Assert.Equal(student.CreatedAt, result.CreatedAt);
        Assert.Equal(student.UpdatedAt, result.UpdatedAt);
        Assert.Equal(student.isActive, result.isActive);
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
        var student = _studentFaker.Generate();
        await _repository.Add(student);

        var result = await _repository.Delete(student.Id);

        Assert.True(result);
        var deletedStudent = await _repository.GetById(student.Id);
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
        var student1 = _studentFaker.Generate();
        var student2 = _studentFaker.Generate();
        await _repository.Add(student1);
        await _repository.Add(student2);

        var result = await _repository.GetAll();

        Assert.Equal(2, result.Count());


        Assert.Contains(result, s =>
            s.FirstName == student1.FirstName &&
            s.LastName == student1.LastName &&
            s.Email == student1.Email &&
            s.Phone == student1.Phone &&
            s.DateOfBirth == student1.DateOfBirth &&
            s.Age == student1.Age &&
            s.CreatedAt == student1.CreatedAt &&
            s.UpdatedAt == student1.UpdatedAt &&
            s.isActive == student1.isActive
        );

        Assert.Contains(result, s =>
            s.FirstName == student2.FirstName &&
            s.LastName == student2.LastName &&
            s.Email == student2.Email &&
            s.Phone == student2.Phone &&
            s.DateOfBirth == student2.DateOfBirth &&
            s.Age == student2.Age &&
            s.CreatedAt == student2.CreatedAt &&
            s.UpdatedAt == student2.UpdatedAt &&
            s.isActive == student2.isActive
        );
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmpty_WhenNoActiveStudents_WorstCase()
    {
        var student1 = _studentFaker.Generate();
        student1.isActive = false;
        await _repository.Add(student1);

        var result = await _repository.GetAll();

        Assert.Empty(result);
    }

    [Fact]
    public async Task FilterStudents_ShouldReturnFilteredStudents_BestCase()
    {
        var student1 = _studentFaker.Generate();
        var student2 = _studentFaker.Generate();
        await _repository.Add(student1);
        await _repository.Add(student2);

        var (students, totalCount) = await _repository.FilterStudents(1, 10, student1.FirstName);

        Assert.Single(students);
        Assert.Equal(1, totalCount);
        Assert.Equal(student1.FirstName, students.First().FirstName);
        Assert.Equal(student1.LastName, students.First().LastName);
        Assert.Equal(student1.Email, students.First().Email);
        Assert.Equal(student1.Phone, students.First().Phone);
        Assert.Equal(student1.DateOfBirth, students.First().DateOfBirth);
        Assert.Equal(student1.Age, students.First().Age);
        Assert.Equal(student1.CreatedAt, students.First().CreatedAt);
        Assert.Equal(student1.UpdatedAt, students.First().UpdatedAt);
        Assert.Equal(student1.isActive, students.First().isActive);
    }

    [Fact]
    public async Task FilterStudents_ShouldReturnEmpty_WhenNoMatches_WorstCase()
    {
        var student1 = _studentFaker.Generate();
        await _repository.Add(student1);

        var (students, totalCount) = await _repository.FilterStudents(1, 10, "NonExistent");

        Assert.Empty(students);
        Assert.Equal(0, totalCount);
    }

    [Fact]
    public async Task Update_ShouldUpdateStudentDetails_WhenStudentExists_BestCase()
    {
        var student = _studentFaker.Generate();
        await _repository.Add(student);

        student.FirstName = "UpdatedFirstName";
        student.LastName = "UpdatedLastName";
        student.Email = "updatedemail@example.com";
        student.Phone = "9998887776";
        student.Age = 25;

        var result = await _repository.Update(student);

        Assert.NotNull(result);
        Assert.Equal("UpdatedFirstName", result.FirstName);
        Assert.Equal("UpdatedLastName", result.LastName);
        Assert.Equal("updatedemail@example.com", result.Email);
        Assert.Equal("9998887776", result.Phone);
        Assert.Equal(25, result.Age);

        var updatedStudent = await _repository.GetById(student.Id);
        Assert.NotNull(updatedStudent);
        Assert.Equal("UpdatedFirstName", updatedStudent.FirstName);
        Assert.Equal("UpdatedLastName", updatedStudent.LastName);
        Assert.Equal("updatedemail@example.com", updatedStudent.Email);
        Assert.Equal("9998887776", updatedStudent.Phone);
        Assert.Equal(25, updatedStudent.Age);
    }

    [Fact]
    public async Task Update_ShouldReturnNull_WhenStudentDoesNotExist_WorstCase()
    {
        var nonExistentStudent = _studentFaker.Generate();
        nonExistentStudent.Id = 999;

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await _repository.Update(nonExistentStudent));
    }
}
