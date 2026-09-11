using MediatR;
using Zukya.Application.UseCases.Users.Common;

namespace Zukya.Application.UseCases.Users.CreateUser;

public interface ICreateUser : IRequestHandler<CreateUserInput, UserTokenOutput>;
