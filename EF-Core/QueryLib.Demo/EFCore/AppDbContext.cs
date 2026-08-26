using Microsoft.EntityFrameworkCore;

namespace QueryLib.Demo.EFCore;

public class AppDbContext : DbContext
{
    public DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5442;Username=postgres;Password=postgres;Database=mohaymen");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>()
            .HasNoKey()
            .ToTable("Student");
    }
}