using MediatR;
using Zukya.Application.UseCases.Users.Common;

namespace Zukya.Application.UseCases.Users.Authenticate;

public interface IAuthenticate : IRequestHandler<AuthenticateInput, UserTokenOutput>;
