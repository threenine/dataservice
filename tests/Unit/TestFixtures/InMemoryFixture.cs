using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using TestDatabase;

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
        context.TestEntities.AddRange(TestEntities());
        context.SaveChanges();
        return context;
    }
    public void Dispose()
    {
        Context?.Dispose();
    }


    private static List<TestEntity> TestEntities()
    {
        BuilderSetup.DisablePropertyNamingFor<TestEntity, Guid>(x => x.Id);

        return Builder<TestEntity>.CreateListOfSize(50)
            .TheFirst(1)
            .With(x => x.Id = Guid.Parse("0E9E4227-E8A5-4BA5-9FC4-48272F778EA0"))
            .Build().ToList();
    }
}