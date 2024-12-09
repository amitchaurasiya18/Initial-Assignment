using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolAPI.Business.Models;

namespace SchoolAPI.Business.Data
{
    public class SchoolAPIReadDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public SchoolAPIReadDbContext(DbContextOptions<SchoolAPIReadDbContext> dbContextOptions) : base(dbContextOptions) { }
    }
}