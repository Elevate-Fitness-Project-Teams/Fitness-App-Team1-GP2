namespace FitnessApp.Shared.Events;

public record UserRegisteredEvent(
    int UserId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber
);
