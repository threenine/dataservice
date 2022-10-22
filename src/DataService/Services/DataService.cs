using System.Data.Common;
using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Threenine.ApiResponse;
using Threenine.Data;

namespace Threenine.Services;
public class DataService<TEntity> : IDataService<TEntity> where TEntity : class
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly IEntityValidationService<TEntity> _validationService;


    public DataService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger, IEntityValidationService<TEntity> validationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _validationService = validationService;
    }

    public async Task<SingleResponse<TResponse>> Create<TDomain, TResponse>(TDomain domain)
      where TResponse : class
        where TDomain : class
    { 
        try
        {
            var entity = _mapper.Map<TEntity>(domain);

            var validateErrors = await _validationService.Validate(entity);

            if (validateErrors.Any()) return new SingleResponse<TResponse>(null, validateErrors.ToList());
            
            var created = _unitOfWork.GetRepository<TEntity>().Insert(entity);
            await _unitOfWork.CommitAsync();
            return new SingleResponse<TResponse>(_mapper.Map<TResponse>(created));
        }
        catch (DbUpdateException e)
        {
            _logger.Error(e, nameof(Create));
            return new SingleResponse<TResponse>(null, new List<KeyValuePair<string, string[]>>()
            {
                new(ErrorKeyNames.Conflict, new[] { "A record already exists" })
            });
        }
        catch (DbException e)
        {
            _logger.Error(e, nameof(Create));
            return new SingleResponse<TResponse>(null, new List<KeyValuePair<string, string[]>>()
            {
                new(ErrorKeyNames.Database, new[] { e.InnerException?.Message })
            });
        }
    }

    public async Task<SingleResponse<TResponse>> Patch<TDomain, TResponse>(
        Expression<Func<TEntity, bool>> predicate,
        JsonPatchDocument<TDomain> domain)
      
        where TDomain : class
        where TResponse : class
    {
        try
        {
            var entity = await _unitOfWork.GetRepositoryAsync<TEntity>().SingleOrDefaultAsync(predicate, enableTracking:true);
            var mapped = _mapper.Map<TDomain>(entity);
            domain.ApplyTo(mapped);
            var patched = _mapper.Map(mapped, entity);
            var validateErrors = await _validationService.Validate(patched);

            if (validateErrors.Any()) return new SingleResponse<TResponse>(null, validateErrors.ToList());
            _unitOfWork.GetRepository<TEntity>().Update(patched);
            await _unitOfWork.CommitAsync();
            var updated = _mapper.Map<TResponse>(patched);
            return new SingleResponse<TResponse>(updated);
        }
        catch (DbUpdateException e)
        {
            _logger.Error(e, nameof(Patch));
            return new SingleResponse<TResponse>(null, new List<KeyValuePair<string, string[]>>()
            {
                new(ErrorKeyNames.Conflict, new[] { "Could not patch record trying to update to existing record value" })
            });
        }
        catch (DbException e)
        {
            _logger.Error(e, nameof(Patch));
            return new SingleResponse<TResponse>(null, new List<KeyValuePair<string, string[]>>()
            {
                new(ErrorKeyNames.Database, new[] { "Could not apply update"})
            });
        }
    }

    public async Task<SingleResponse<TResponse>> Update<TDomain, TResponse>(
        Expression<Func<TEntity, bool>> predicate, TDomain domain)
        where TDomain : class
        where TResponse : class
    {
        try
        {
            var entity = await _unitOfWork.GetRepositoryAsync<TEntity>().SingleOrDefaultAsync(predicate, enableTracking: true);
            var updated = _mapper.Map(domain, entity);
            var validateErrors = await _validationService.Validate(entity);
            if (validateErrors.Any()) return new SingleResponse<TResponse>(null, validateErrors.ToList());
            _unitOfWork.GetRepository<TEntity>().Update(updated);
            await _unitOfWork.CommitAsync();
            var result = _mapper.Map<TResponse>(updated);
            return new SingleResponse<TResponse>(result);
        }
        catch (DbUpdateException ex)
        {
            var poo = ex.Message;
            return new SingleResponse<TResponse>(null, new List<KeyValuePair<string, string[]>>()
            {
                new(ErrorKeyNames.Conflict, new[] { "Could not update record" })
            });
        }
        catch (DbException e)
        {
            _logger.Error(e, nameof(Update));
            return new SingleResponse<TResponse>(null, new List<KeyValuePair<string, string[]>>()
            {
                new(ErrorKeyNames.Database, new[] { "Could not update record" })
            });
        }
        catch (Exception e)
        {
            _logger.Error(e, nameof(Update));
            return new SingleResponse<TResponse>(null, new List<KeyValuePair<string, string[]>>()
            {
                new(ErrorKeyNames.Database, new[] { "An error occurred"})
            });
        }
    }

   
}
