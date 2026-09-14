using MediatR;
using Zukya.Application.Common.Exceptions;
using Zukya.Application.UseCases.Users.Common;
using Zukya.Domain.Users.Entities;
using Zukya.Domain.Users.Repositories;

namespace Zukya.Application.UseCases.Users.GetUser;

public class GetUser : IRequestHandler<GetUserInput, UserOutput>
{
    private readonly IUserRepository userRepository;

    public GetUser(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<UserOutput> Handle(GetUserInput input, CancellationToken cancellationToken)
    {
        User user = await userRepository.GetById(input.Id, cancellationToken);

        if (user == null)
            throw new NotFoundException($"User {input.Id} not found");

        return UserOutput.ToOutput(user);
    }
}
