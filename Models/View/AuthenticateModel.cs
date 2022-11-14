namespace TinTucGameAPI.Models.View
{
    public class AuthenticateModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Salt { get; set; }
    }
}
