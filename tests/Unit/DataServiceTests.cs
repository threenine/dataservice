using AutoMapper;
using FizzWare.NBuilder;
using FluentValidation;
using Serilog;
using Moq;
using Shouldly;
using TestDatabase;
using Threenine.Data;
using Threenine.Services;
using Threenine.TestFixtures;
using Xunit;

namespace Threenine;

[Collection(GlobalTestStrings.TestEntityCollectionName)]
public class DataServiceTests
{
    private readonly InMemoryFixture _fixture;
    private readonly DataService _classUnderTest;
    

    private IUnitOfWork _unitOfWork;
    private IMapper _mapper;
    private Mock<ILogger> _loggerMock;
    private Mock<IEnumerable<IValidator>> _validatorMock;
    public DataServiceTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
        _unitOfWork = new UnitOfWork<TestDbContext>(_fixture.Context);
        _loggerMock = new Mock<ILogger>();
        _validatorMock = new Mock<IEnumerable<IValidator>>();
        
        var mapperConfiguration = new MapperConfiguration(configuration => configuration.AddProfile<TestMapping>());
        mapperConfiguration.AssertConfigurationIsValid();
        _mapper = mapperConfiguration.CreateMapper();

        _classUnderTest = new DataService(_unitOfWork, _mapper, _loggerMock.Object, _validatorMock.Object );
    }


    /*
    [Fact]
    public async Task ShouldUpdate()
    {
        var test = Builder<TestDTOs>.CreateNew()
            .With(x => x.Id = Guid.Parse("0E9E4227-E8A5-4BA5-9FC4-48272F778EA0"))
            .With(x => x.Name = "Test Update")
            .Build();

      
        var result = await _classUnderTest.Update<TestEntity, TestDTOs, TestResponse>(x => x.Id == test.Id, test);
        
        result.ShouldSatisfyAllConditions(
            () => result.ShouldNotBeNull(),
            () => result.Item.ShouldBeOfType<TestResponse>()
            );
    }*/
    
  
    
}