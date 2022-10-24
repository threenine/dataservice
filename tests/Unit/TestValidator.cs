
using TestDatabase;

namespace Threenine;

public class TestValidator : InlineValidator<TestEntity>
{
    public TestValidator() 
    {

    }
    public TestValidator(params Action<TestValidator>[] actions)
    {
        foreach (var action in actions)
        {
            action(this);
        }
    }
}