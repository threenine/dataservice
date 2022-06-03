using Microsoft.EntityFrameworkCore;

namespace Threenine.TestFixtures;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }
}