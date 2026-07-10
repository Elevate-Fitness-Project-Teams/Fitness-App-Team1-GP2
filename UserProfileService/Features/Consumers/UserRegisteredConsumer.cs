using FitnessApp.Shared.Events;
using MassTransit;
using MediatR;
using System.Threading.Tasks;
using UserProfileService.Features.Commands.CreateUserProfile;

namespace UserProfileService.Features.Consumers
{
    public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
    {
        private readonly IMediator _mediator;

        public UserRegisteredConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            var message = context.Message;

            var command = new CreateUserProfileCommand
            {
                UserId = message.UserId,
                FirstName = message.FirstName,
                LastName = message.LastName,
                Email = message.Email,
                PhoneNumber = message.PhoneNumber
            };

            await _mediator.Send(command);
        }
    }
}
