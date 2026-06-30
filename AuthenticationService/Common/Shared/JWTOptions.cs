namespace AuthenticationService.Common.Shared
{
    public class JWTOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public int DurationInDays { get; set; }

    }
}
