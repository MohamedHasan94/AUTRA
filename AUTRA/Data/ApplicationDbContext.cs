using System;
using System.Collections.Generic;
using System.Text;
using AUTRA.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AUTRA.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            /*Composite key for Project (UserId, ProjectName)*/
            builder.Entity<Project>()
                     .HasKey(p => new { p.Fk_UserId, p.Name });
        }
        public virtual DbSet<Project> Projects { get; set; }
    }
}
