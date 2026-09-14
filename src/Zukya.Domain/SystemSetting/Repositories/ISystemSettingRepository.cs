using DomainEntity = Zukya.Domain.SystemSetting.Entities;

namespace Zukya.Domain.SystemSetting.Repositories;

public interface ISystemSettingRepository
{
    Task Save(DomainEntity.SystemSetting aggregate, CancellationToken cancellationToken);
    Task<DomainEntity.SystemSetting> GetById(int id, CancellationToken cancellationToken);
    Task<DomainEntity.SystemSetting?> GetByKey(string key, CancellationToken cancellationToken);
    Task<IEnumerable<DomainEntity.SystemSetting>> GetAll(CancellationToken cancellationToken);
    Task Delete(DomainEntity.SystemSetting aggregate, CancellationToken cancellationToken);
}
