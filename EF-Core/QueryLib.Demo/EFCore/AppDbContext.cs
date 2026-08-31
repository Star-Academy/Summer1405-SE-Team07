using Microsoft.EntityFrameworkCore;

namespace QueryLib.Demo.EFCore;

public class AppDbContext : DbContext
{
	private readonly string _connectionString;

	public AppDbContext(string connectionString)
	{
		_connectionString = connectionString;
	}	

    public DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql( _connectionString)
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>()
            .HasNoKey()
            .ToTable("Student");
    }
}