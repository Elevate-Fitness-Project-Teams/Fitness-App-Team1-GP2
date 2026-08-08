namespace AuthenticationService.Domain.Events
{
    public record ProfileLifecycleInitiatedEvent(
        int UserId,
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber
    );
}
