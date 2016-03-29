using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.MicrosoftAccount;
using Newtonsoft.Json.Linq;
using Sport.Service.Models;
using Sport.Service.Results;
using Sport.Service.Utilities.Auth;
using Sport.Shared;

namespace Sport.Service.Controllers
{
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        private ApplicationUserManager _userManager;

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }
        public LoginController()
        {
        }
        public LoginController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;

            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        public async Task<IHttpActionResult> Post(LoginSession request)
        {
            try
            {
                ExternalLoginData externalLogin = null;
                switch (request.LoginProvider)
                {
                    case LoginProvider.Microsoft:
                        externalLogin = await MicrosoftLogin(request); break;
                    case LoginProvider.Google:
                        externalLogin = await GoogleLogin(request); break;
                    case LoginProvider.Facebook:
                        externalLogin = await FacebookLogin(request);
                        break;
                    case LoginProvider.Local:
                        return await LoginLocal(request);
                }


                if (externalLogin == null)
                {
                    return InternalServerError();
                }

                var loginInfo = new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey);

                ApplicationUser user = await UserManager.FindAsync(loginInfo);

                if (user == null)
                {
                    user = AuthHelper.CreateUserAsync(externalLogin.ExternalIdentity, request.LoginProvider.ToString());
                    var result = await UserManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        return InternalServerError();
                    }

                    result = await UserManager.AddLoginAsync(user.Id, loginInfo);

                    if (!result.Succeeded)
                    {
                        return InternalServerError();
                    }
                    await AuthHelper.SendConfirmEmail(this, UserManager, user);
                }
               
                return new ApiResult(await AuthHelper.SignIn(UserManager, user));

            }
            catch (Exception ex)
            {
                return new ApiResult(ex);

            }

        }


        private async Task<IHttpActionResult> LoginLocal(LoginSession session)
        {
            ApplicationUser user = await UserManager.FindByEmailAsync(session.Email);

            if (user == null || !UserManager.CheckPassword(user, session.Password))
            {
                return BadRequest();
            }

            return new ApiResult(await AuthHelper.SignIn(UserManager, user));
        }

        private async Task<ExternalLoginData> MicrosoftLogin(LoginSession request)
        {
            var userRequest = request.User == null
                ? await MicrosoftAccountHelper.GetMicrosoftUser(request.AccessToken)
                : JObject.FromObject(request.User);

            var context =
                await
                    MicrosoftAccountHelper.UpdateContext(
                        new MicrosoftAccountAuthenticatedContext(Request.GetOwinContext(), userRequest,
                            request.AccessToken, request.RefreshToken, request.Expires)
                        {
                            Identity = new ClaimsIdentity(Startup.OAuthOptions.AuthenticationType)
                        });

            return ExternalLoginData.FromIdentity(context.Identity);
        }

        private async Task<ExternalLoginData> FacebookLogin(LoginSession request)
        {
            var userRequest = request.User == null
                ? await FacebookHelper.GetFacebookUser(request.AccessToken)
                : JObject.FromObject(request.User);

            var context =
                await
                    FacebookHelper.UpdateContext(
                        new FacebookAuthenticatedContext(Request.GetOwinContext(), userRequest,
                            request.AccessToken, request.Expires)
                        {
                            Identity = new ClaimsIdentity(Startup.OAuthOptions.AuthenticationType)
                        });

            return ExternalLoginData.FromIdentity(context.Identity);
        }

        private async Task<ExternalLoginData> GoogleLogin(LoginSession request)
        {
            var userRequest = request.User == null
                ? await GoogleHelper.GetGoogleUser(request.AccessToken)
                : JObject.FromObject(request.User);
            var token = JObject.FromObject(request.TokenResponse);
            GoogleOAuth2AuthenticatedContext context = new GoogleOAuth2AuthenticatedContext(Request.GetOwinContext(),
                userRequest, token)
            {
                Identity = new ClaimsIdentity(Startup.OAuthOptions.AuthenticationType)
            };
            await GoogleHelper.UpdateContext(context);

            return ExternalLoginData.FromIdentity(context.Identity);
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; private set; }
            public string ProviderKey { get; private set; }
            public string UserName { get; private set; }
            public ClaimsIdentity ExternalIdentity { get; private set; }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                Claim providerKeyClaim = identity?.FindFirst(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(providerKeyClaim?.Issuer) || string.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    ExternalIdentity = identity,
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }


        }
    }
}

