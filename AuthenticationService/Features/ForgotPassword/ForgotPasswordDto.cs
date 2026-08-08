namespace AuthenticationService.Features.ForgotPassword
{
    public class ForgotPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public int OtpExpiresIn { get; set; }
        public int CanResendIn { get; set; }
    }
}
