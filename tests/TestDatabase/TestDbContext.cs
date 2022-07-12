using Microsoft.EntityFrameworkCore;

namespace TestDatabase;
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

            entity.HasKey(x => x.Id).Metadata.IsPrimaryKey();
            
            entity.HasIndex(x => x.Id).IsUnique();
            
            entity.Property(e => e.Id);
            entity.Property(e => e.Name);
            
        });
    }
}