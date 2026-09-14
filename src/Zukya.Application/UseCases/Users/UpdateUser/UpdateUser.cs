using MediatR;
using Zukya.Application.Common.Exceptions;
using Zukya.Application.Common.Interfaces;
using Zukya.Application.UseCases.Users.Common;
using Zukya.Domain.Users.Entities;
using Zukya.Domain.Users.Repositories;

namespace Zukya.Application.UseCases.Users.UpdateUser;

public class UpdateUser : IRequestHandler<UpdateUserInput, UserOutput>
{
    private readonly IUserRepository userRepository;
    private readonly IUnitOfWork unitOfWork;

    public UpdateUser(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        this.userRepository = userRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<UserOutput> Handle(UpdateUserInput input, CancellationToken cancellationToken)
    {
        User user = await userRepository.GetById(input.Id, cancellationToken);

        NotFoundException.ThrowIfNull(user, $"User with id {input.Id} not found.");

        if (!string.IsNullOrEmpty(input.Phone))
        {
            User userWithSamePhone = await userRepository.GetByPhone(input.Phone, cancellationToken);

            ConflictException.ThrowIfNotNull(
                userWithSamePhone,
                $"Phone '{input.Phone}' is already registered."
            );
        }

        user.Update(input.Name, input.Phone, input.TaxId);

        await userRepository.Update(user, cancellationToken);
        await unitOfWork.Commit(cancellationToken);

        return UserOutput.ToOutput(user);
    }
}
