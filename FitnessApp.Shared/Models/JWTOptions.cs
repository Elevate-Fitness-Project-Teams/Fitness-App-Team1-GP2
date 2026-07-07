namespace FitnessApp.Shared.Models
{
    public class JWTOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public int AccessTokenDurationInMinutes { get; set; }
        public int RefreshTokenDurationInDays { get; set; }
    }
}
