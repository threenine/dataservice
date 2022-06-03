using System.Linq.Expressions;
using Microsoft.AspNetCore.JsonPatch;
using Threenine.ApiResponse;

namespace Threenine;

public interface IDataService
{
    Task<SingleResponse<TResponse>> Create<TEntity, TResponse>(TEntity entity) where TEntity : class
        where TResponse : class;


    Task<SingleResponse<TResponse>> Patch<TEntity, TDomain, TResponse>(Expression<Func<TEntity, bool>> predicate,
        JsonPatchDocument<TDomain> domain) where TEntity : class
        where TResponse : class
        where TDomain : class;


    Task<SingleResponse<TResponse>> Update<TEntity, TDomain, TResponse>(Expression<Func<TEntity, bool>> predicate,
        TDomain domain)
        where TEntity : class
        where TDomain : class
        where TResponse : class;
}