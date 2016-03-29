using System;
using System.Linq;

namespace Sport.Shared
{
    public class SocialInfo
    {
        public LoginProvider Provider { get; set; }
        /// <summary>
        /// your OAuth2 client id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The scopes for the particular API you're accessing, delimited by "+" symbols
        /// </summary>
        public string Scope
        {
            get
            {
                switch (Provider)
                {
                    case LoginProvider.Facebook:
                        //TODO Aggregate
                        break;
                    case LoginProvider.Google:
                        return Scopes.Aggregate((x, y) => x + "+" + y);
                    case LoginProvider.Microsoft:
                        //TODO Aggregate
                        break;
                    case LoginProvider.Local:
                       break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// The auth URL for the service
        /// </summary>
        public Uri AuthorizeUrl { get; set; }

        /// <summary>
        /// The redirect URL for the service
        /// </summary>
        public Uri RedirectUrl { get; set; }

        public string UserInfoApi { get; set; }
        public string ClientSecret { get; set; }
        public string[] Scopes { get; set; }
    }
}