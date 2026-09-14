using MediatR;
using Zukya.Application.Common.Pagination;
using Zukya.Domain.Shared.Repositories;

namespace Zukya.Application.UseCases.Users.ListUsers;

public class ListUsersInput : PaginatedListInput<string>, IRequest<ListUsersOutput>
{
    public ListUsersInput(
        int page = 1,
        int perPage = 15,
        string search = "",
        string sort = "",
        SearchOrder dir = SearchOrder.Asc
    ) : base(page, perPage, search, sort, dir)
    {
    }

    public ListUsersInput()
        : base(1, 15, "", "", SearchOrder.Asc)
    {
    }
}
