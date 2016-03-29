using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Sport.Service.Models;
using Sport.Service.Providers;

namespace Sport.Service.Utilities.Auth
{
    public static class AuthHelper
    {
        public static  Task<bool> SendConfirmSms(ApplicationUserManager userManager, ApplicationUser user, string phoneNumber)
        {
            //try
            //{
            //    var phoneCode = await userManager.GenerateChangePhoneNumberTokenAsync(user.Id, phoneNumber);
            //    await userManager.SmsService.SendAsync(new IdentityMessage()
            //    {
            //        Body = string.Format("Sử dụng mã {0} để kích hoạt tài khoản", phoneCode),
            //        Destination = phoneNumber
            //    });
            //    return true;
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return Task.FromResult(false);
        }
        public static  Task SendResetSms(ApiController controller, ApplicationUserManager userManager, ApplicationUser user)
        {
            //var code = await userManager.GenerateChangePhoneNumberTokenAsync(user.Id, user.PhoneNumber);
            //await userManager.SmsService.SendAsync(new IdentityMessage()
            //{
            //    Body = string.Format("Sử dụng mã {0} để khôi phục mật khẩu",code),
            //    Destination = user.PhoneNumber
            //});
            return Task.FromResult((object)null);
        }
        public static  Task SendResetEmail(ApiController controller, ApplicationUserManager userManager, ApplicationUser user)
        {
            //var code = await userManager.GenerateChangePhoneNumberTokenAsync(user.Id, user.PhoneNumber);

            //var emailService = userManager.EmailService as EmailService;
            //if (emailService != null)
            //    await emailService.SendResetEmailAsync(user, code);

            return Task.FromResult((object)null);
        }
        public static Task SendConfirmEmail(ApiController controller, ApplicationUserManager userManager, ApplicationUser user)
        {
            //var code = await userManager.GenerateEmailConfirmationTokenAsync(user.Id);
            //var callbackUrl = controller.Url.Link("Manage", new { controller = "Account", action = "ConfirmEmail", userId = user.Id, code = code });

            //var emailService = userManager.EmailService as EmailService;
            //if (emailService != null)
            //    await emailService.SendWelcomeEmailAsync(user, callbackUrl);

            return Task.FromResult((object)null);
        }

        public static Task<object> Token(string username)
        {
            var identity = new ClaimsIdentity(Startup.OAuthOptions.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, username));
            var properties = ApplicationOAuthProvider.CreateProperties(username);
            var ticket = new AuthenticationTicket(identity, properties);
            var currentUtc = new SystemClock().UtcNow;
            ticket.Properties.IssuedUtc = currentUtc;
            ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromHours(1));
            var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);
            return Task.FromResult((object)(new { access_token = accessToken, expires = ticket.Properties.ExpiresUtc }));
        }

        public static async Task<object> SignIn(ApplicationUserManager userManager, ApplicationUser user)
        {
            var oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
            oAuthIdentity.AddClaim(new Claim("urn:account:PhoneNumberConfirmed", user.PhoneNumberConfirmed.ToString()));
            var properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
            var ticket = new AuthenticationTicket(oAuthIdentity, properties);
            var currentUtc = new SystemClock().UtcNow;
            ticket.Properties.IssuedUtc = currentUtc;
            ticket.Properties.ExpiresUtc = currentUtc.Add(Startup.OAuthOptions.AccessTokenExpireTimeSpan);
            var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);
            var data = new
            {
                user.Id,
                user.Firstname,
                user.Lastname,
                user.Email,
                user.Dob,
                user.Gender,
                user.AvatarUrl,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                user.EmailConfirmed,
                AccessToken = accessToken,
                Expires = ticket.Properties.ExpiresUtc,
                Roles = userManager.GetRoles(user.Id)
            };

            return data;
        }

        public static bool TryAddClaim(this ClaimsIdentity identity, Claim claim)
        {
            if (!identity.HasClaim(claim.Type, claim.Value))
            {
                identity.AddClaim(claim);
                return true;
            }
            return false;
        }

        public static bool TryAddClaim(this ClaimsIdentity identity, string claimType, string value, string valueType, string issuer)
        {
            if (!identity.HasClaim(claimType, value))
            {
                identity.AddClaim(new Claim(claimType, value, valueType, issuer));
                return true;
            }
            return false;
        }

        public static ApplicationUser CreateUserAsync(ClaimsIdentity externalIdentity, string loginProvider)
        {
            var email = externalIdentity.FindFirst(ClaimTypes.Email).Value;
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                Firstname = externalIdentity.FindFirstValue(ClaimTypes.GivenName),
                Lastname = externalIdentity.FindFirstValue(ClaimTypes.Surname),
                EmailConfirmed = false,
                AvatarUrl = externalIdentity.FindFirstValue("urn:account:image"),
            };

            var g = externalIdentity.FindFirstValue(ClaimTypes.Gender);
            bool gender;
            bool.TryParse(g, out gender);
            user.Gender = gender;

            var d = externalIdentity.FindFirstValue(ClaimTypes.DateOfBirth);
            DateTime date;
            if (DateTime.TryParseExact(d, "MM/dd/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out date))
                user.Dob = date;

            //var t = externalIdentity.FindFirstValue("urn:account:timezone");
            //var timeZone = 0;
            //if (int.TryParse(t, out timeZone))
            //{
            //    user.TimeZone = timeZone;
            //}

            return user;
        }

        public static void AddRole(string roleName, ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // Check to see if Role Exists, if not create it
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));
            }
        }

        public static void AddUser(ApplicationDbContext context, string username, string email, string password)
        {
            if (!context.Users.Any(u => u.UserName == username))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = username, Email = email };

                manager.Create(user, password);
                manager.AddToRole(user.Id, "Admin");
            }
        }
    }
}
