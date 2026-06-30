namespace AuthenticationService.Features.Authentication.Login
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool ProfileCompleted { get; set; }
        public bool IsPremium { get; set; }
    }
}
