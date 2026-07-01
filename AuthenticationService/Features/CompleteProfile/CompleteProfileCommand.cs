using MediatR;

namespace AuthenticationService.Features.CompleteProfile
{
    public record CompleteProfileCommand(int UserId) : IRequest;
}
