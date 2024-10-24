using AutoMapper;
using Moq;
using SchoolAPI.DTO;
using SchoolAPI.Models;
using SchoolAPI.Repository.Interfaces;
using SchoolAPI.Services;

namespace School_API_xUnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void TestGetStudentByIdMethod_ShouldReturnStudentOfId()
        {
            var studentGet = new StudentGetDTO
            {
                FirstName = "Mock",
                LastName = "Test1",
                Email = "mocktest1@gmail.com",
                Phone = "9348938483",
                DateOfBirth = new DateTime(1976, 10, 20),
                Age = 21,
            };

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
            studentRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(student);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<StudentGetDTO>(student)).Returns(studentGet);

            var studentService = new StudentService(studentRepositoryMock.Object, mapperMock.Object);
            var result = studentService.GetById(1);
            Assert.NotNull(result);
            Assert.Equal(student.FirstName, result.FirstName);
            Assert.Equal(student.LastName, result.LastName);
            Assert.Equal(student.Phone, result.Phone);
        }

        [Fact]
        public void TestGetStudentByIdMethod_ShouldReturnNull()
        {
            var studentRepositoryMock = new Mock<IStudentRepository>();
            studentRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(() => null);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<StudentGetDTO>(null)).Returns(() => null);

            var studentService = new StudentService(studentRepositoryMock.Object, mapperMock.Object);
            var result = studentService.GetById(1);
            Assert.Null(result);
        }


        [Fact]
        public void TestGetAllStudentsMethod_ShouldReturnAllStudents()
        {
            var getAllStudentsDTO = new List<StudentGetDTO>
            {
               new StudentGetDTO
               {
                   FirstName = "Mock",
                   LastName = "Test1",
                   Email = "mocktest1@gmail.com",
                   Phone = "9348938483", DateOfBirth = new DateTime(2003,06,08),
                   Age = 21
               },
               new StudentGetDTO
               {
                   FirstName = "Mock",
                   LastName = "Test2",
                   Email = "mocktest2@gmail.com",
                   Phone = "9348938494",
                   DateOfBirth = new DateTime(1976,10,20),
                   Age = 48
               }
            };

            var getAllStudents = new List<Student>
            {
                new Student{
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
                new Student{
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
            studentRepositoryMock.Setup(x => x.GetAll()).Returns(getAllStudents);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<IEnumerable<StudentGetDTO>>(getAllStudents)).Returns(getAllStudentsDTO);

            var studentService = new StudentService(studentRepositoryMock.Object, mapperMock.Object);
            var result = studentService.GetAll();
            Assert.NotNull(result);
            // Assert.Equal(getAllStudents, result);
            Assert.Same(getAllStudentsDTO, result);
        }


        [Fact]
        public void TestAddStudent_ShouldReturnAddedStudentDTO()
        {
            var addStudentPost = new StudentPostDTO
            {
                FirstName = "Mock",
                LastName = "Test1",
                Email = "mocktest1@gmail.com",
                Phone = "9348938483",
                DateOfBirth = new DateTime(1976, 10, 20)
            };

            var student = new Student
            {
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
            studentRepositoryMock.Setup(x => x.Add(It.IsAny<Student>())).ReturnsAsync(student);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<Student>(addStudentPost)).Returns(student);

            var studentService = new StudentService(studentRepositoryMock.Object, mapperMock.Object);
            var result = studentService.Add(addStudentPost);
            Assert.NotNull(result);
            Assert.Equal(addStudentPost.FirstName, result.FirstName);
            Assert.Equal(addStudentPost.LastName, result.LastName);
            Assert.Same(addStudentPost, result);
        }

        [Fact]
        public async Task TestUpdateStudent_ShouldReturnUpdatedStudent()
        {
            var studentPostDTO = new StudentPostDTO
            {
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
            studentRepositoryMock.Setup(x => x.GetById(1)).Returns(existingStudent);
            studentRepositoryMock.Setup(x => x.Update(It.IsAny<Student>())).ReturnsAsync(existingStudent);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<Student>(studentPostDTO)).Returns(existingStudent);

            var studentService = new StudentService(studentRepositoryMock.Object, mapperMock.Object);
            var result = await studentService.Update(1, studentPostDTO);

            Assert.NotNull(result);
            Assert.Equal(studentPostDTO.FirstName, result.FirstName);
            Assert.Equal(studentPostDTO.LastName, result.LastName);
        }

        [Fact]
        public async Task TestDeleteStudent_ShouldReturnSuccessMessage()
        {
            var studentId = 1;

            var studentRepositoryMock = new Mock<IStudentRepository>();
            studentRepositoryMock.Setup(x => x.Delete(studentId)).ReturnsAsync("Student deleted successfully");

            var studentService = new StudentService(studentRepositoryMock.Object, null);
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

            var studentService = new StudentService(studentRepositoryMock.Object, null);
            var result = await studentService.Delete(studentId);

            Assert.NotNull(result);
            Assert.Equal("Student not found", result);
        }

        [Fact]
        public async Task TestFilterStudents_ShouldReturnFilteredStudents()
        {
            // Arrange
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
            var filteredStudentsDTO = new List<StudentGetDTO>
            {
                new StudentGetDTO
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "johndoe@gmail.com",
                    Phone = "1234567890",
                    DateOfBirth = student1.DateOfBirth,
                    Age = student1.Age
                },
                new StudentGetDTO
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "janedoe@gmail.com",
                    Phone = "0987654321",
                    DateOfBirth = student2.DateOfBirth,
                    Age = student2.Age
                }
            };

            var studentRepositoryMock = new Mock<IStudentRepository>();
            studentRepositoryMock.Setup(x => x.FilterStudents(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((students, 2));

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<IEnumerable<StudentGetDTO>>(students)).Returns(filteredStudentsDTO);

            var studentService = new StudentService(studentRepositoryMock.Object, mapperMock.Object);

            // Act
            var result = await studentService.FilterStudents(1, 10, "");

            Assert.NotNull(result);
            Assert.Equal(2, ((IEnumerable<StudentGetDTO>)result.GetType().GetProperty("Students").GetValue(result)).Count());
            Assert.Equal(2, result.GetType().GetProperty("TotalCount").GetValue(result));
            Assert.Equal(filteredStudentsDTO, (IEnumerable<StudentGetDTO>)result.GetType().GetProperty("Students").GetValue(result));
        }


        [Fact]
        public async Task TestFilterStudents_WithSearchTerm_ShouldReturnFilteredStudentsByAge()
        {
            // Arrange
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

            var filteredStudentsDTO = new List<StudentGetDTO>
            {
                new StudentGetDTO
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "janedoe@gmail.com",
                    Phone = "0987654321",
                    DateOfBirth = student2.DateOfBirth,
                    Age = student2.Age
                }
            };

            var studentRepositoryMock = new Mock<IStudentRepository>();
            studentRepositoryMock
                .Setup(x => x.FilterStudents(It.IsAny<int>(), It.IsAny<int>(), "22"))
                .ReturnsAsync((new List<Student> { student2 }, 1));

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<IEnumerable<StudentGetDTO>>(It.IsAny<IEnumerable<Student>>())).Returns(filteredStudentsDTO);

            var studentService = new StudentService(studentRepositoryMock.Object, mapperMock.Object);

            var result = await studentService.FilterStudents(1, 10, "22");

            Assert.NotNull(result);
            var studentsResult = (IEnumerable<StudentGetDTO>)result.GetType().GetProperty("Students").GetValue(result);
            Assert.Single(studentsResult);
            Assert.Equal("Jane", studentsResult.First().FirstName);
            Assert.Equal(1, result.GetType().GetProperty("TotalCount").GetValue(result));
        }
    }
}