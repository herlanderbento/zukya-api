using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Zukya.Domain.SystemSetting.Repositories;
using DomainEntity = Zukya.Domain.SystemSetting.Entities;

namespace Zukya.Infra.Persistence.Modules.SystemSetting.Repositories;

public class SystemSettingRepository(DatabaseContext context, IMemoryCache cache) : ISystemSettingRepository
{
    private DbSet<DomainEntity.SystemSetting> SystemSettings => context.Set<DomainEntity.SystemSetting>();

    public Task Save(DomainEntity.SystemSetting aggregate, CancellationToken cancellationToken)
    {
        var cacheKey = $"system_setting_{aggregate.Key}";

        cache.Remove(cacheKey);

        SystemSettings.Update(aggregate);
        return Task.CompletedTask;
    }

    public async Task<DomainEntity.SystemSetting> GetById(int id, CancellationToken cancellationToken)
    {
        DomainEntity.SystemSetting? model = await SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return model!;
    }

    public async Task<DomainEntity.SystemSetting?>
        GetByKey(string key, CancellationToken cancellationToken)
    {
        var cacheKey = $"system_setting_{key}";

        if (cache.TryGetValue(cacheKey, out DomainEntity.SystemSetting? cachedModel)) return cachedModel;

        DomainEntity.SystemSetting? model = await SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Key == key, cancellationToken);

        if (model == null)
            return model;

        MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

        cache.Set(cacheKey, model, cacheOptions);

        return model;
    }

    public async Task<IEnumerable<DomainEntity.SystemSetting>> GetAll(CancellationToken cancellationToken) =>
        await SystemSettings.ToListAsync(cancellationToken);


    public Task Delete(DomainEntity.SystemSetting aggregate, CancellationToken cancellationToken)
    {
        var cacheKey = $"system_setting_{aggregate.Key}";

        cache.Remove(cacheKey);

        SystemSettings.Remove(aggregate);
        return Task.CompletedTask;
    }
}
