namespace FitnessApp.Shared.Events
{
    public record UserProfileUpdatedEvent(
        int UserId,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string ProfilePictureUrl
    );
}
