using Zukya.Domain.Roles.Entities;
using Zukya.Domain.Shared.Repositories;

namespace Zukya.Domain.Roles.Repositories;

public interface IPermissionRepository : IRepository<Permission>, ISearchableRepository<Permission, string>;
