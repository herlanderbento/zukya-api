using Zukya.Application.Common.Exceptions;
using Zukya.Application.Common.Interfaces;
using Zukya.Application.UseCases.Users.Common;
using Zukya.Domain.Users.Entities;
using Zukya.Domain.Users.Repositories;

namespace Zukya.Application.UseCases.Users.Authenticate;

public class Authenticate : IAuthenticate
{
    private readonly IUserRepository userRepository;
    private readonly ICryptography cryptography;
    private readonly ITokenProvider tokenProvider;

    public Authenticate(
        IUserRepository userRepository,
        ICryptography cryptography,
        ITokenProvider tokenProvider)
    {
        this.userRepository = userRepository;
        this.cryptography = cryptography;
        this.tokenProvider = tokenProvider;
    }

    public async Task<UserTokenOutput> Handle(AuthenticateInput input, CancellationToken cancellationToken)
    {
        User user = await userRepository.GetByEmailOrPhone(input.Login, cancellationToken);

        WrongCredentialsException.ThrowIfNull(user, "login or password incorrect.");

        var isPasswordValid = await cryptography.Verify(
            input.Password,
            user.Password!,
            cancellationToken
        );

        WrongCredentialsException.ThrowIfNull(isPasswordValid, "login or password incorrect.");

        TokenResponse token = await tokenProvider.GenerateToken(user);

        return new UserTokenOutput(
            token.AccessToken,
            UserOutput.ToOutput(user)
        );
    }
}
