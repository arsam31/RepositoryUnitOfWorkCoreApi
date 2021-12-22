using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApiRepositoryUnitOfWork.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreApiRepositoryUnitOfWork.DatabaseContext
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public virtual DbSet<User> User{ get; set; }
    }
}
