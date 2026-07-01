namespace AuthenticationService.Domain.Contracts
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        (bool Verified, bool RehashNeeded, string? NewHash) VerifyPassword(string password, string hashedPassword);
    }
}
