using MediatR;
using Zukya.Application.Common.Exceptions;
using Zukya.Application.Common.Interfaces;
using Zukya.Domain.Users.Entities;
using Zukya.Domain.Users.Repositories;

namespace Zukya.Application.UseCases.Users.ChangeUserStatus;

public class ChangeUserStatus : IRequestHandler<ChangeUserStatusInput>
{
    private readonly IUserRepository userRepository;
    private readonly IUnitOfWork unitOfWork;

    public ChangeUserStatus(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        this.userRepository = userRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task Handle(ChangeUserStatusInput input, CancellationToken cancellationToken)
    {
        User user = await userRepository.GetById(input.Id, cancellationToken);

        NotFoundException.ThrowIfNull(user, $"User with id {input.Id} not found.");

        user.ChangeStatus(input.Status);

        await userRepository.Update(user, cancellationToken);
        await unitOfWork.Commit(cancellationToken);
    }
}
