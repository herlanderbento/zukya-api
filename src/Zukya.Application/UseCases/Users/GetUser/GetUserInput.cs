using MediatR;
using Zukya.Application.UseCases.Users.Common;

namespace Zukya.Application.UseCases.Users.GetUser;

public class GetUserInput(Guid id) : IRequest<UserOutput>
{
    public Guid Id { get; set; } = id;
}
