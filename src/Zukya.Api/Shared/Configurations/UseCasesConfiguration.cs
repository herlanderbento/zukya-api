using FluentValidation;
using Zukya.Application.Common.Behaviors;
using Zukya.Application.Common.Events;
using Zukya.Application.Common.Interfaces;
using Zukya.Application.UseCases.Users.Authenticate;
using Zukya.Application.UseCases.Users.CreateUser;
using Zukya.Domain.Shared.Events;
using Zukya.Domain.SystemSetting.Repositories;
using Zukya.Domain.Users.Repositories;
using Zukya.Infra.Persistence;
using Zukya.Infra.Persistence.Modules.SystemSetting.Repositories;
using Zukya.Infra.Persistence.Modules.Users.Repositories;
using Zukya.Infra.Services.Auth;

namespace Zukya.Api.Shared.Configurations;

public static class UseCasesConfiguration
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<CreateUserInput>();
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddRepositories();
        services.AddValidators();
        services.AddDomainEvents();
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddTransient<HttpClient>(sp =>
            sp.GetRequiredService<IHttpClientFactory>().CreateClient()
        );
        // Repositories
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IVerificationCodeRepository, VerificationCodeRepository>();
        services.AddTransient<ISystemSettingRepository, SystemSettingRepository>();
        // Unit of work
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        // Providers
        services.AddTransient<ICryptography, BCryptHasher>();
        services.AddTransient<ITokenProvider, JwtTokenService>();
        // Services
        services.AddOptions();

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        // User validators
        services.AddValidatorsFromAssemblyContaining<CreateUserInputValidator>();
        services.AddValidatorsFromAssemblyContaining<AuthenticateInputValidator>();
        return services;
    }

    private static IServiceCollection AddDomainEvents(this IServiceCollection services)
    {
        services.AddTransient<IDomainEventPublisher, DomainEventPublisher>();

        return services;
    }
}
