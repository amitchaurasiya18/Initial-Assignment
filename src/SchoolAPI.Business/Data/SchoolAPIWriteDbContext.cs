using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolAPI.Business.Models;

namespace SchoolAPI.Business.Data
{
    public class SchoolAPIWriteDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public SchoolAPIWriteDbContext(DbContextOptions<SchoolAPIWriteDbContext> dbContextOptions) : base(dbContextOptions) { }
    }
}