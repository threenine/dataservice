using System.Data.Common;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Threenine.ApiResponse;
using Threenine.Data;

namespace Threenine.Services;
public class DataService : IDataService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
        

    public DataService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SingleResponse<TResponse>> Create<TEntity, TDomain, TResponse>(TDomain domain)
        where TEntity : class
        where TResponse : class
    where TDomain : class
    {
        try
        {
            var entity = _mapper.Map<TEntity>(domain);
            var created = _unitOfWork.GetRepository<TEntity>().Insert(entity);
            await _unitOfWork.CommitAsync();
            return new SingleResponse<TResponse>(_mapper.Map<TResponse>(created));
        }
        catch (DbUpdateException e)
        {
            _logger.Error(e, nameof(DataService.Create));
            return new SingleResponse<TResponse>(null, new List<KeyValuePair<string, string[]>>()
            {
                new(ErrorKeyNames.Conflict, new[] { "A record already exists" })
            });
        }
        catch (DbException e)
        {
            _logger.Error(e, nameof(DataService.Create));
            return new SingleResponse<TResponse>(null, new List<KeyValuePair<string, string[]>>()
            {
                new(ErrorKeyNames.Database, new[] { e.InnerException?.Message })
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
            var entity = await _unitOfWork.GetRepositoryAsync<TEntity>().SingleOrDefaultAsync(predicate, enableTracking:true);
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
            _logger.Error(e, nameof(DataService.Patch));
            return new SingleResponse<TResponse>(null, new List<KeyValuePair<string, string[]>>()
            {
                new(ErrorKeyNames.Conflict, new[] { "Could not patch record trying to update to existing record value" })
            });
        }
        catch (DbException e)
        {
            _logger.Error(e, nameof(DataService.Patch));
            return new SingleResponse<TResponse>(null, new List<KeyValuePair<string, string[]>>()
            {
                new(ErrorKeyNames.Database, new[] { "Could not apply update"})
            });
        }
    }

    public async Task<SingleResponse<TResponse>> Update<TEntity, TDomain, TResponse>(
        Expression<Func<TEntity, bool>> predicate, TDomain domain) where TEntity : class
        where TDomain : class
        where TResponse : class
    {
        try
        {
            var entity = await _unitOfWork.GetRepositoryAsync<TEntity>().SingleOrDefaultAsync(predicate, enableTracking: true);
            var updated = _mapper.Map(domain, entity);
            _unitOfWork.GetRepository<TEntity>().Update(updated);
            await _unitOfWork.CommitAsync();
            var result = _mapper.Map<TResponse>(updated);
            return new SingleResponse<TResponse>(result);
        }
        catch (DbUpdateException e)
        {
            return new SingleResponse<TResponse>(null, new List<KeyValuePair<string, string[]>>()
            {
                new(ErrorKeyNames.Conflict, new[] { "Could not update record" })
            });
        }
        catch (DbException e)
        {
            _logger.Error(e, nameof(DataService.Update));
            return new SingleResponse<TResponse>(null, new List<KeyValuePair<string, string[]>>()
            {
                new(ErrorKeyNames.Database, new[] { "Could not update record" })
            });
        }
        catch (Exception e)
        {
            _logger.Error(e, nameof(DataService.Update));
            return new SingleResponse<TResponse>(null, new List<KeyValuePair<string, string[]>>()
            {
                new(ErrorKeyNames.Database, new[] { "An error occurred"})
            });
        }
    }
}
