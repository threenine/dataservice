using Microsoft.EntityFrameworkCore;
using Unit.TestFixtures;

namespace Threenine.TestFixtures;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }
    public virtual DbSet<TestEntity> TestEntities { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestEntity>(entity =>
        {
            entity.ToTable(nameof(TestEntity));

            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Id);

            entity.Property(e => e.Name);
            entity.Property(e => e.Id);
        });
    }
}