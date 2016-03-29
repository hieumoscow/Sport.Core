namespace Sport.Shared
{
    public class LoginSession
    {
        public LoginProvider LoginProvider { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Expires { get; set; }
        public object TokenResponse { get; set; }
        public object User { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }        
    }
}