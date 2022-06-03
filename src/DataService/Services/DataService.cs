using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Threenine.ApiResponse;
using Threenine.Data;

namespace Threenine.Services;
public class DataService : IDataService
{
     private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DataService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SingleResponse<TResponse>> Create<TEntity, TResponse>(TEntity entity)
        where TEntity : class
        where TResponse : class
    {
        try
        {
            var created = _unitOfWork.GetRepository<TEntity>().Insert(entity);
            await _unitOfWork.CommitAsync();
            return new SingleResponse<TResponse>(_mapper.Map<TResponse>(created));
        }
        catch (DbUpdateException e)
        {
            return new SingleResponse<TResponse>(null, new List<KeyValuePair<string, string[]>>()
            {
                new(ErrorKeyNames.Conflict, new[] { e.InnerException?.Message })
            });
        }
    }

    public async Task<SingleResponse<TResponse>> Patch<TEntity, TDomain, TResponse>(
        Expression<Func<TEntity, bool>> predicate,
        JsonPatchDocument<TDomain> domain)
        where TEntity : class
        where TDomain : class
        where TResponse : class
    {
        try
        {
            var entity = _unitOfWork.GetRepository<TEntity>().SingleOrDefault(predicate, enableTracking: true);
            var mapped = _mapper.Map<TDomain>(entity);
            domain.ApplyTo(mapped);
            var patched = _mapper.Map(mapped, entity);

            _unitOfWork.GetRepository<TEntity>().Update(patched);
            await _unitOfWork.CommitAsync();
            var updated = _mapper.Map<TResponse>(patched);
            return new SingleResponse<TResponse>(updated);
        }
        catch (DbUpdateException e)
        {
            return new SingleResponse<TResponse>(null, new List<KeyValuePair<string, string[]>>()
            {
                new(ErrorKeyNames.Conflict, new[] { e.InnerException?.Message })
            });
        }
    }

    public async Task<SingleResponse<TResponse>> Update<TEntity, TDomain, TResponse>(
        Expression<Func<TEntity, bool>> predicate, TDomain domain) where TEntity : class
        where TDomain : class
        where TResponse : class
    {
        var entity = _unitOfWork.GetRepository<TEntity>().SingleOrDefault(predicate, enableTracking: true);
        var updated = _mapper.Map(domain, entity);
        _unitOfWork.GetRepository<TEntity>().Update(updated);
        await _unitOfWork.CommitAsync();
        var result = _mapper.Map<TResponse>(updated);
        return new SingleResponse<TResponse>(result);
    }
}
