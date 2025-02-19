﻿using Microsoft.EntityFrameworkCore;
namespace TimeSheet.Models
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Timesheet>Timesheets { get; set; }
        public DbSet<Project> Projects { get; set; } // DbSet for Projects
        public DbSet<UserProjectAssignment> UserProjectAssignments { get; set; } // New Table

    }
}
