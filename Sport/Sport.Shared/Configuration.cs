using System;

namespace Sport.Shared
{
    public static class Configuration
    {
       
        static Configuration()
        {
            //TODO: Change To Product
            MicrosoftConfig = new SocialInfo
            {
                Provider = LoginProvider.Microsoft,
                ClientId = "#",
                ClientSecret = "#",
                Scopes = new[] { "wl.emails", "wl.basic", "wl.offline_access", "wl.signin", "wl.birthday" },
                AuthorizeUrl = new Uri("https://login.live.com/oauth20_authorize.srf"),
                RedirectUrl = new Uri("https://login.live.com/oauth20_desktop.srf"),
                UserInfoApi = "https://apis.live.net/v5.0/me"
            };

            FacebookConfig = new SocialInfo
            {
                Provider = LoginProvider.Facebook,
                ClientId = "#",
                ClientSecret = "#",
                Scopes = new[] { "email", "public_profile" },
                AuthorizeUrl = new Uri("https://m.facebook.com/dialog/oauth/"),
                RedirectUrl = new Uri("https://localhost:44300/signin-facebook"),
                UserInfoApi = "https://graph.facebook.com/me"
            };

            GoogleConfig = new SocialInfo
            {
                Provider = LoginProvider.Google,
                ClientId = "#",
                ClientSecret = "#",
                Scopes = new[] { "https://www.googleapis.com/auth/userinfo.email" },
                AuthorizeUrl = new Uri("https://accounts.google.com/o/oauth2/auth"),
                RedirectUrl = new Uri("https://localhost:44300/signin-google", UriKind.Absolute)
            };

        }

        public static SocialInfo MicrosoftConfig { get; private set; }
        public static SocialInfo GoogleConfig { get; private set; }
        public static SocialInfo FacebookConfig { get; private set; }
    }
}
