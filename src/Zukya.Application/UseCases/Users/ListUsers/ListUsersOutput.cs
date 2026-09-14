using Zukya.Application.Common.Pagination;
using Zukya.Application.UseCases.Users.Common;

namespace Zukya.Application.UseCases.Users.ListUsers;

public class ListUsersOutput(int page, int perPage, int total, IReadOnlyList<UserOutput> items)
    : PaginatedListOutput<UserOutput>(page, perPage, total, items);
