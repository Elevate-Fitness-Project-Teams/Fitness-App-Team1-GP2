using MediatR;

namespace AuthenticationService.Features.Authentication.CompleteProfile
{
    public record CompleteProfileCommand(int UserId) : IRequest;
}
