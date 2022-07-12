using Xunit;

namespace Threenine.TestFixtures;

[CollectionDefinition(GlobalTestStrings.TestEntityCollectionName)]
public class TestEntityCollection : ICollectionFixture<InMemoryFixture>
{
    
}