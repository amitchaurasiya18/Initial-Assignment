using Moq;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Repository.Interfaces;
using SchoolAPI.Business.Services;

namespace School_API_xUnitTest
{
    public class SchoolAPIServiceTests
    {
        [Fact]
        public async Task TestGetStudentByIdMethod_ShouldReturnStudentOfId()
        {
            var student = new Student
            {
                Id = 1,
                FirstName = "Mock",
                LastName = "Test1",
                Email = "mocktest1@gmail.com",
                Phone = "9348938483",
                DateOfBirth = new DateTime(1976, 10, 20),
                Age = 21,
                isActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            var studentRepositoryMock = new Mock<IStudentRepository>();
            studentRepositoryMock.Setup(x => x.GetById(1)).ReturnsAsync(student);

            var studentService = new StudentService(studentRepositoryMock.Object);
            var result = await studentService.GetById(1);
            
            Assert.NotNull(result);
            Assert.Equal(student.FirstName, result.FirstName);
            Assert.Equal(student.LastName, result.LastName);
            Assert.Equal(student.Phone, result.Phone);
        }

        [Fact]
        public async Task TestGetStudentByIdMethod_ShouldReturnNull()
        {
            var studentRepositoryMock = new Mock<IStudentRepository>();
            studentRepositoryMock.Setup(x => x.GetById(1)).ReturnsAsync((Student)null);

            var studentService = new StudentService(studentRepositoryMock.Object);
            var result = await studentService.GetById(1);
            Assert.Null(result);
        }

        [Fact]
        public async Task TestGetAllStudentsMethod_ShouldReturnAllStudents()
        {
            var getAllStudents = new List<Student>
            {
                new Student
                {
                    Id = 1,
                    FirstName = "Mock",
                    LastName = "Test1",
                    Email = "mocktest1@gmail.com",
                    Phone = "9348938483",
                    DateOfBirth = new DateTime(2003, 06, 06),
                    Age = 21,
                    isActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                },
                new Student
                {
                    Id = 2,
                    FirstName = "Mock",
                    LastName = "Test2",
                    Email = "mocktest2@gmail.com",
                    Phone = "9348938494",
                    DateOfBirth = new DateTime(1976, 10, 20),
                    Age = 48,
                    isActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                }
            };

            var studentRepositoryMock = new Mock<IStudentRepository>();
            studentRepositoryMock.Setup(x => x.GetAll()).ReturnsAsync(getAllStudents);

            var studentService = new StudentService(studentRepositoryMock.Object);
            var result = await studentService.GetAll();
            
            Assert.NotNull(result);
            Assert.Equal(getAllStudents.Count, result.Count());
            Assert.Equal(getAllStudents[0].FirstName, result.ElementAt(0).FirstName);
            Assert.Equal(getAllStudents[1].FirstName, result.ElementAt(1).FirstName);
        }

        [Fact]
        public async Task TestAddStudent_ShouldReturnAddedStudent()
        {
            var student = new Student
            {
                FirstName = "Mock",
                LastName = "Test1",
                Email = "mocktest1@gmail.com",
                Phone = "9348938483",
                DateOfBirth = new DateTime(1976, 10, 20)
            };

            var addedStudent = new Student
            {
                Id = 1,
                FirstName = "Mock",
                LastName = "Test1",
                Email = "mocktest1@gmail.com",
                Phone = "9348938483",
                DateOfBirth = new DateTime(1976, 10, 20),
                Age = 48,
                isActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            var studentRepositoryMock = new Mock<IStudentRepository>();
            studentRepositoryMock.Setup(x => x.Add(It.IsAny<Student>())).ReturnsAsync(addedStudent);

            var studentService = new StudentService(studentRepositoryMock.Object);
            var result = await studentService.Add(student);
            
            Assert.NotNull(result);
            Assert.Equal(addedStudent.FirstName, result.FirstName);
            Assert.Equal(addedStudent.LastName, result.LastName);
        }

        [Fact]
        public async Task TestUpdateStudent_ShouldReturnUpdatedStudent()
        {
            var student = new Student
            {
                Id = 1,
                FirstName = "UpdatedMock",
                LastName = "Test1",
                Email = "updatedmocktest1@gmail.com",
                Phone = "9348938483",
                DateOfBirth = new DateTime(1976, 10, 20)
            };

            var existingStudent = new Student
            {
                Id = 1,
                FirstName = "Mock",
                LastName = "Test1",
                Email = "mocktest1@gmail.com",
                Phone = "9348938483",
                DateOfBirth = new DateTime(1976, 10, 20),
                Age = 48,
                isActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            var studentRepositoryMock = new Mock<IStudentRepository>();
            studentRepositoryMock.Setup(x => x.GetById(1)).ReturnsAsync(existingStudent);
            studentRepositoryMock.Setup(x => x.Update(existingStudent)).ReturnsAsync(existingStudent);

            var studentService = new StudentService(studentRepositoryMock.Object);
            var result = await studentService.Update(1, student);
            
            Assert.NotNull(result);
            Assert.Equal(student.FirstName, result.FirstName);
            Assert.Equal(student.LastName, result.LastName);
        }

        [Fact]
        public async Task TestDeleteStudent_ShouldReturnSuccessMessage()
        {
            var studentId = 1;

            var studentRepositoryMock = new Mock<IStudentRepository>();
            studentRepositoryMock.Setup(x => x.Delete(studentId)).ReturnsAsync("Student deleted successfully");

            var studentService = new StudentService(studentRepositoryMock.Object);
            var result = await studentService.Delete(studentId);
            
            Assert.NotNull(result);
            Assert.Equal("Student deleted successfully", result);
        }

        [Fact]
        public async Task TestDeleteStudent_ShouldReturnNotFoundMessage()
        {
            var studentId = 1;

            var studentRepositoryMock = new Mock<IStudentRepository>();
            studentRepositoryMock.Setup(x => x.Delete(studentId)).ReturnsAsync("Student not found");

            var studentService = new StudentService(studentRepositoryMock.Object);
            var result = await studentService.Delete(studentId);
            
            Assert.NotNull(result);
            Assert.Equal("Student not found", result);
        }

        [Fact]
        public async Task TestFilterStudents_ShouldReturnFilteredStudents()
        {
            var student1 = new Student
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@gmail.com",
                Phone = "1234567890",
                DateOfBirth = new DateTime(2003, 06, 08),
                Age = 21,
                isActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            var student2 = new Student
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Doe",
                Email = "janedoe@gmail.com",
                Phone = "0987654321",
                DateOfBirth = new DateTime(2002, 05, 10),
                Age = 22,
                isActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            var students = new List<Student> { student1, student2 };

            var studentRepositoryMock = new Mock<IStudentRepository>();
            studentRepositoryMock.Setup(x => x.FilterStudents(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((students, students.Count));

            var studentService = new StudentService(studentRepositoryMock.Object);

            var result = await studentService.FilterStudents(1, 10, "");

            Assert.NotNull(result);
            Assert.Equal(students, result.Students.ToList());
            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task TestFilterStudents_WithSearchTerm_ShouldReturnFilteredStudentsByAge()
        {
            var student1 = new Student
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@gmail.com",
                Phone = "1234567890",
                DateOfBirth = new DateTime(2003, 06, 08),
                Age = 21,
                isActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            var student2 = new Student
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Doe",
                Email = "janedoe@gmail.com",
                Phone = "0987654321",
                DateOfBirth = new DateTime(2002, 05, 10),
                Age = 22,
                isActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            var students = new List<Student> { student1, student2 };

            var studentRepositoryMock = new Mock<IStudentRepository>();
            studentRepositoryMock
                .Setup(x => x.FilterStudents(It.IsAny<int>(), It.IsAny<int>(), "22"))
                .ReturnsAsync((new List<Student> { student2 }, 1));

            var studentService = new StudentService(studentRepositoryMock.Object);

            var result = await studentService.FilterStudents(1, 10, "22");
            Assert.Single(result.Students);
            Assert.Equal("Jane", result.Students.First().FirstName);
            Assert.Equal(1, result.TotalCount);
        }
    }
}
