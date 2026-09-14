using Zukya.Application.Common.Exceptions;
using Zukya.Application.Common.Interfaces;
using Zukya.Application.UseCases.Users.Common;
using Zukya.Domain.Users.Entities;
using Zukya.Domain.Users.Enums;
using Zukya.Domain.Users.Repositories;
using UserRole = Zukya.Domain.Users.Enums.UserRole;

namespace Zukya.Application.UseCases.Users.CreateUser;

public class CreateUser : ICreateUser
{
    private readonly IUserRepository userRepository;
    private readonly IVerificationCodeRepository verificationCodeRepository;
    private readonly ICryptography cryptography;
    private readonly ITokenProvider tokenProvider;
    private readonly IUnitOfWork unitOfWork;

    public CreateUser(
        IUserRepository userRepository,
        IVerificationCodeRepository verificationCodeRepository,
        ICryptography cryptography,
        ITokenProvider tokenProvider,
        IUnitOfWork unitOfWork)
    {
        this.userRepository = userRepository;
        this.verificationCodeRepository = verificationCodeRepository;
        this.cryptography = cryptography;
        this.tokenProvider = tokenProvider;
        this.unitOfWork = unitOfWork;
    }

    public async Task<UserTokenOutput> Handle(CreateUserInput input, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(input.Email))
        {
            User userWithSameEmail = await userRepository.GetByEmail(
                input.Email,
                cancellationToken
            );
            
            ConflictException.ThrowIfNotNull(
                userWithSameEmail,
                $"Email '{input.Email}' is already registered."
            );
        }

        if (!string.IsNullOrEmpty(input.Phone))
        {
            User userWithSamePhone = await userRepository.GetByPhone(input.Phone, cancellationToken);

            ConflictException.ThrowIfNotNull(
                userWithSamePhone,
                $"Phone '{input.Phone}' is already registered."
            );
        }

        var hashedPassword = await cryptography.HashPassword(input.Password, cancellationToken);

        var user = new User(
            input.Name,
            input.Email,
            input.Phone,
            hashedPassword,
            input.Role == UserRole.Seller ? 1 : 0,
            input.TaxId
        );

        await userRepository.Insert(user, cancellationToken);

        var code = VerificationCode.Create(
            user.Id,
            VerificationType.AccountVerification,
            TimeSpan.FromMinutes(15)
        );

        await verificationCodeRepository.Insert(code, cancellationToken);

        TokenResponse token = await tokenProvider.GenerateToken(user);

        await unitOfWork.Commit(cancellationToken);

        return new UserTokenOutput(
            token.AccessToken,
            UserOutput.ToOutput(user)
        );
    }
}
