using Microsoft.EntityFrameworkCore;
using SchoolAPI.Models;

namespace SchoolAPI.Data
{
    public class SchoolAPIDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; } 
        public SchoolAPIDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }
    }
}
