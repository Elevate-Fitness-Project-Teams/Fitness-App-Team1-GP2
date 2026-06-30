namespace AuthenticationService.Features.Authentication.Register
{
    public class RegisterDto
    {
        public int UserId { get; set; }
        public bool RequiresProfileCompletion { get; set; } = true;
    }
}
