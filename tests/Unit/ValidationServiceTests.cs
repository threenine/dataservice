using FluentValidation;
using Moq;
using TestDatabase;
using Threenine.Services;
using Xunit;

namespace Threenine;

public class ValidationServiceTests
{
    private TestValidator _validator;

    public ValidationServiceTests()
    {
        _validator = new TestValidator();
    }

    [Fact]
    public async Task ShouldValidate()
    {
        var testEntity = new TestEntityValidator();

        var service = new EntityValidationService<TestEntity>(new Mock<IEnumerable<IValidator<TestEntity>>>().Object);




    }
}