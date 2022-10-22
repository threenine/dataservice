using FluentValidation;

namespace Threenine.Services;

public class EntityValidationService<TEntity> : IEntityValidationService<TEntity> where TEntity : class
{
    private readonly IEnumerable<IValidator<TEntity>> _validators;
    
    public EntityValidationService(IEnumerable<IValidator<TEntity>> validators)
    {
        _validators = validators;
    }
    
    public async Task<Dictionary<string,string[]>> Validate(TEntity entity) 
    {
        var context = new ValidationContext<TEntity>(entity);
        var result = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context)));

        return  result.SelectMany(r => r.Errors)
            .Where(f => f != null)
            .GroupBy(x => x.PropertyName,
                x => x.ErrorMessage,
                (propertyName, errorMessages) => new
                {
                    Key = propertyName,
                    Values = errorMessages.Distinct().ToArray()
                })
            .ToDictionary(x => x.Key, x => x.Values);
    }
}