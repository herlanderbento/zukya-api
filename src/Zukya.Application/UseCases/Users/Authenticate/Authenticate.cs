using Zukya.Application.Common.Exceptions;
using Zukya.Application.Common.Interfaces;
using Zukya.Application.UseCases.Users.Common;
using Zukya.Domain.SystemSetting.Entities;
using Zukya.Domain.SystemSetting.Repositories;
using Zukya.Domain.Users.Entities;
using Zukya.Domain.Users.Enums;
using Zukya.Domain.Users.Repositories;

namespace Zukya.Application.UseCases.Users.Authenticate;

public class Authenticate : IAuthenticate
{
    private readonly IUserRepository userRepository;
    private readonly ISystemSettingRepository systemSettingRepository;  
    private readonly ICryptography cryptography;
    private readonly ITokenProvider tokenProvider;
    private readonly IUnitOfWork unitOfWork;

    public Authenticate(
        IUserRepository userRepository,
        ISystemSettingRepository systemSettingRepository,
        ICryptography cryptography,
        ITokenProvider tokenProvider,
        IUnitOfWork unitOfWork
    )
    {
        this.userRepository = userRepository;
        this.systemSettingRepository = systemSettingRepository;
        this.cryptography = cryptography;
        this.tokenProvider = tokenProvider;
        this.unitOfWork = unitOfWork;
    }

    public async Task<UserTokenOutput> Handle(AuthenticateInput input, CancellationToken cancellationToken)
    {
        User user = await userRepository.GetByEmailOrPhone(input.Login, cancellationToken);

        UnauthorizedException.ThrowIfNull(user, "login or password incorrect.");

        if (user.Status is UserStatus.Banned)
            ForbiddenException.Throw("Access denied. Your account has been banned.");
        
        var isPasswordValid = await cryptography.Verify(
            input.Password,
            user.Password!,
            cancellationToken
        );

        if (!isPasswordValid)
        {
            SystemSetting? enabledSetting = await systemSettingRepository
                .GetByKey(
                    "security.auto_banned_on_failed_login.enabled",
                    cancellationToken
                );
            var isAutoBanEnabled = enabledSetting != null && bool.TryParse(enabledSetting.Value, out var enabled) &&
                                   enabled;

            if (isAutoBanEnabled)
            {
                SystemSetting? maxAttemptsSetting =
                    await systemSettingRepository.GetByKey("security.max_failed_login_attempts", cancellationToken);
                var maxAttempts = maxAttemptsSetting != null && int.TryParse(maxAttemptsSetting.Value, out var attempts)
                    ? attempts
                    : 5;

                user.RegisterFailedLogin(maxAttempts);

                await userRepository.Update(user, cancellationToken);
                await unitOfWork.Commit(cancellationToken);
            }

            UnauthorizedException.ThrowIfFalse(isPasswordValid, "login or password incorrect.");
        }

        if (user.FailedLoginAttempts > 0)
        {
            user.ResetFailedLoginAttempts();
            await userRepository.Update(user, cancellationToken);
            await unitOfWork.Commit(cancellationToken);
        }
        
        TokenResponse token = await tokenProvider.GenerateToken(user);

        return new UserTokenOutput(
            token.AccessToken,
            UserOutput.ToOutput(user)
        );
    }
}
