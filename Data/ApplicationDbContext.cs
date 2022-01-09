using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NextGen_Snacky.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NextGen_Snacky.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Category> Category { get ; set ; }
    }
}
