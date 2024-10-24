using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SchoolAPI.Data;
using SchoolAPI.Models;
using SchoolAPI.Repository.Interfaces;

namespace SchoolAPI.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly SchoolAPIDbContext _schoolAPIDbContext;
        public StudentRepository(SchoolAPIDbContext schoolAPIDbContext) 
        {
            _schoolAPIDbContext = schoolAPIDbContext;
        }

        public async Task<Student> Add(Student student)
        {
            await _schoolAPIDbContext.Students.AddAsync(student);
            await _schoolAPIDbContext.SaveChangesAsync();
            return student;
        }

        public async Task<string> Delete(int id)
        {
            var student = await _schoolAPIDbContext.Students.FirstOrDefaultAsync(s => s.isActive ==true && s.Id == id);
            
            if (student == null)
            {
                return "Student not found";
            }
            student.isActive = false;
            _schoolAPIDbContext.SaveChanges();
            return "Student deleted successfully";
        }

        public async Task<(IEnumerable<Student>, int TotalCount)> FilterStudents(int page, int pageSize, string searchTerm)
        {
            IQueryable<Student> query = _schoolAPIDbContext.Students.AsQueryable();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(s => s.isActive == true);
            }
            else
            {
                query = query.Where(s =>
                    (s.FirstName.Contains(searchTerm) ||
                     s.LastName.Contains(searchTerm) ||
                     s.Email.Contains(searchTerm) ||
                     s.Age.ToString().Contains(searchTerm))
                     && s.isActive == true);
            }

            var totalCount = await query.CountAsync();

            var students = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (students, totalCount);
        }

        public IEnumerable<Student> GetAll()
        {
            return _schoolAPIDbContext.Students.Where(s => s.isActive == true).ToList();
        }

        public Student GetById(int id)
        {
            return _schoolAPIDbContext.Students.FirstOrDefault(s => s.Id == id && s.isActive == true);
        }

        public async Task<Student> Update(Student student)
        {
            _schoolAPIDbContext.Update(student);
            await _schoolAPIDbContext.SaveChangesAsync();
            return student;
        }
    }
}
