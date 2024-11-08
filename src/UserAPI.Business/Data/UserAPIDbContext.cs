using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserAPI.Business.Models;

namespace UserAPI.Business.Data
{
    public class UserAPIDbContext:DbContext
    {
        public DbSet<User> Users { get; set; } 
        public UserAPIDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }
    }
}