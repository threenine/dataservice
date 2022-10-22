using FluentValidation;

namespace TestDatabase;

public class TestEntityValidator : AbstractValidator<TestEntity>
{
    public TestEntityValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}