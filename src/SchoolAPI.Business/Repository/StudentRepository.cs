using Microsoft.EntityFrameworkCore;
using SchoolAPI.Business.Data;
using SchoolAPI.Business.Models;
using SchoolAPI.Business.Repository.Interfaces;

namespace SchoolAPI.Business.Repository
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

        public async Task<bool> Delete(int id)
        {
            Student? student = await _schoolAPIDbContext.Students.FirstOrDefaultAsync(s => s.IsActive ==true && s.Id == id) ?? new Student();
            student.IsActive = false;
            _schoolAPIDbContext.SaveChanges();
            return true;
        }

        public async Task<(IEnumerable<Student>, int TotalCount)> FilterStudents(int page, int pageSize, string searchTerm)
        {
            IQueryable<Student> query = _schoolAPIDbContext.Students.AsQueryable();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(s => s.IsActive == true);
            }
            else
            {
                query = query.Where(s =>
                    (s.FirstName.Contains(searchTerm) ||
                     s.LastName.Contains(searchTerm) ||
                     s.Email.Contains(searchTerm) ||
                     s.Phone.Contains(searchTerm))
                     && s.IsActive == true);
            }

            var totalCount = await query.CountAsync();

            var students = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (students,totalCount);
        }

        public async Task<IEnumerable<Student>> GetAll()
        {
            return await _schoolAPIDbContext.Students.Where(s => s.IsActive == true).ToListAsync();
        }

        public async Task<Student> GetByEmail(string email)
        {
            Student student = await _schoolAPIDbContext.Students.FirstOrDefaultAsync(s => s.IsActive == true && s.Email == email);
            return student;
        }

        public async Task<Student?> GetById(int id)
        {
            Student? student = await _schoolAPIDbContext.Students.FirstOrDefaultAsync(s => s.Id == id && s.IsActive == true);
            return student;
        }

        public async Task<Student> Update(Student student)
        {
            _schoolAPIDbContext.Update(student);
            await _schoolAPIDbContext.SaveChangesAsync();
            return student;
        }
    }
}
