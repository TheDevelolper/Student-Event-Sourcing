using Microsoft.EntityFrameworkCore;

namespace event_sourcing.Infrastructure;

public class EventStoreDbContext : DbContext
{
    public DbSet<StoredEvent> Events => Set<StoredEvent>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=myuser;Password=mypassword;Database=mydb");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StoredEvent>(entity =>
        {
            entity.ToTable("student_events");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Data).HasColumnType("jsonb");
        });
    }
}
