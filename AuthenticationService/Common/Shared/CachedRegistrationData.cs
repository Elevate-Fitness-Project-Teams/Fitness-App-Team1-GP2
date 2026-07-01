namespace AuthenticationService.Common.Shared
{
    public record CachedRegistrationData(
        string FirstName, 
        string LastName, 
        string Email, 
        string PhoneNumber
    );
}
