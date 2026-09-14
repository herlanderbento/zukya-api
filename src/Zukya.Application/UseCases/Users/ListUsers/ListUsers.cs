using MediatR;
using Zukya.Application.UseCases.Users.Common;
using Zukya.Domain.Shared.Repositories;
using Zukya.Domain.Users.Entities;
using Zukya.Domain.Users.Repositories;

namespace Zukya.Application.UseCases.Users.ListUsers;

public class ListUsers : IRequestHandler<ListUsersInput, ListUsersOutput>
{
    private readonly IUserRepository userRepository;

    public ListUsers(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<ListUsersOutput> Handle(ListUsersInput input, CancellationToken cancellationToken)
    {
        SearchOutput<User> searchOutput = await userRepository
            .Search(
                input.ToSearchInput(),
                cancellationToken
            );

        return await ToOutput(searchOutput);
    }

    private static Task<ListUsersOutput> ToOutput(SearchOutput<User> searchOutput)
    {
        return Task.FromResult(
            new ListUsersOutput(
                searchOutput.CurrentPage,
                searchOutput.PerPage,
                searchOutput.Total,
                searchOutput.Items.Select(UserOutput.ToOutput).ToList()
            )
        );
    }
}
