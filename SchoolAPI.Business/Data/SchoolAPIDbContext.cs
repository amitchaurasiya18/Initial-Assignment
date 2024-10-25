using Microsoft.EntityFrameworkCore;
using SchoolAPI.Business.Models;

namespace SchoolAPI.Business.Data
{
    public class SchoolAPIDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; } 
        public SchoolAPIDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }
    }
}
