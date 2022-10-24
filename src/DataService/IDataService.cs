using System.Linq.Expressions;
using Microsoft.AspNetCore.JsonPatch;
using Threenine.ApiResponse;

namespace Threenine;

public interface IDataService<TEntity> where TEntity : class
{
    Task<SingleResponse<TResponse>> Create<TDomain, TResponse>(TDomain domain) 
        where TResponse : class
        where TDomain : class;

    Task<SingleResponse<TResponse>> Patch<TDomain, TResponse>(Expression<Func<TEntity, bool>> predicate,
        JsonPatchDocument<TDomain> domain)
        where TResponse : class
        where TDomain : class;

    Task<SingleResponse<TResponse>> Update<TDomain, TResponse>(Expression<Func<TEntity, bool>> predicate,
        TDomain domain)
        where TDomain : class
        where TResponse : class;
}