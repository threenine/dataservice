using Microsoft.EntityFrameworkCore;

namespace Threenine.TestFixtures;

public class InMemoryFixture : IDisposable
{
    public TestDbContext Context => InMemoryContext();


    private static TestDbContext InMemoryContext()
    {
      
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        var context = new TestDbContext(options);
        context.ChangeTracker.AutoDetectChangesEnabled = true;
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        return context;
    }
    public void Dispose()
    {
        Context?.Dispose();
    }
}