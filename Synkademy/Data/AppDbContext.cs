using Microsoft.EntityFrameworkCore;
using Synkademy.Models;

namespace Synkademy.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ResearchArea> ResearchAreas => Set<ResearchArea>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<ProjectInterest> ProjectInterests => Set<ProjectInterest>();
        public DbSet<SupervisorResearchArea> SupervisorResearchAreas => Set<SupervisorResearchArea>();
        public DbSet<ProjectResearchArea> ProjectResearchAreas => Set<ProjectResearchArea>();
        public DbSet<ProjectTag> ProjectTags => Set<ProjectTag>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Composite Keys for Junction Tables (All EF Core needs!)
            modelBuilder.Entity<ProjectResearchArea>()
                .HasKey(x => new { x.ProjectId, x.ResearchAreaId });
            // Map to existing DB table name (phpMyAdmin shows plural/lowercase names)
            modelBuilder.Entity<ProjectResearchArea>().ToTable("projectresearchareas");

            modelBuilder.Entity<SupervisorResearchArea>()
                .HasKey(x => new { x.SupervisorId, x.ResearchAreaId });
            modelBuilder.Entity<SupervisorResearchArea>().ToTable("supervisorresearchareas");

            modelBuilder.Entity<ProjectTag>()
                .HasKey(x => new { x.ProjectId, x.TagId });
            modelBuilder.Entity<ProjectTag>().ToTable("projecttags");

            // 2. Your Team's Original Mappings
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Student)
                .WithMany(s => s.Projects)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Supervisor)
                .WithMany(e => e.SupervisedProjects)
                .HasForeignKey(p => p.SupervisorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ProjectInterest>()
                .HasOne(pi => pi.Project)
                .WithMany(p => p.Interests)
                .HasForeignKey(pi => pi.ProjectId);

            modelBuilder.Entity<ProjectInterest>()
                .HasOne(pi => pi.Supervisor)
                .WithMany(e => e.Interests)
                .HasForeignKey(pi => pi.SupervisorId);

            // 3. Your Team's Indexes
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email).IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email).IsUnique();

            modelBuilder.Entity<ResearchArea>()
                .HasIndex(r => r.Name).IsUnique();

            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Name).IsUnique();

        }

    }
}