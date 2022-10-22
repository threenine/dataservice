namespace Threenine;

public interface IEntityValidationService<in TEntity> where TEntity : class
{
    Task<Dictionary<string, string[]>> Validate(TEntity entity);
}