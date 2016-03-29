using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.MicrosoftAccount;
using Newtonsoft.Json.Linq;

namespace Sport.Service.Utilities.Auth
{
    /// <summary>
    /// Contains static methods that allow to extract user's information from a <see cref="JObject"/>
    /// instance retrieved from Google after a successful authentication process.
    /// </summary>
    public static class MicrosoftAccountHelper
    {
        public static async Task<MicrosoftAccountAuthenticatedContext> UpdateContext(MicrosoftAccountAuthenticatedContext context)
        {
            JObject payload = context.User;

            if (context.User == null)
            {
                payload = await GetMicrosoftUser(context.AccessToken);
            }

            var identifier = GetId(payload);
            if (!string.IsNullOrEmpty(identifier))
            {

                context.Identity.TryAddClaim(new Claim(ClaimTypes.NameIdentifier, identifier, ClaimValueTypes.String, "Microsoft"));
                context.Identity.TryAddClaim(new Claim("urn:microsoftaccount:id", identifier, ClaimValueTypes.String));
            }
            context.Identity.AddClaim(new Claim("urn:account:image",string.Format("https://apis.live.net/v5.0/{0}/picture", identifier), ClaimValueTypes.String));

            var name = GetName(payload);
            if (!string.IsNullOrEmpty(name))
            {
                context.Identity.TryAddClaim(new Claim(ClaimTypes.Name, name, ClaimValueTypes.String));
                context.Identity.TryAddClaim(new Claim("urn:microsoftaccount:name", name, ClaimValueTypes.String));
            }

            var email = GetEmail(payload);
            if (!string.IsNullOrEmpty(email))
            {
                context.Identity.TryAddClaim(new Claim(ClaimTypes.Email, email, ClaimValueTypes.String));
            }

            var firstName = GetFirstName(payload);
            if (!string.IsNullOrEmpty(firstName))
            {
                context.Identity.TryAddClaim(new Claim(ClaimTypes.GivenName, firstName, ClaimValueTypes.String));
            }

            var lastName = GetLastName(payload);
            if (!string.IsNullOrEmpty(lastName))
            {
                context.Identity.TryAddClaim(new Claim(ClaimTypes.Surname, lastName, ClaimValueTypes.String));
            }
            var birthDay = payload.Value<string>("birth_day");
            var birthMonth = payload.Value<string>("birth_month");
            var birthYear = payload.Value<string>("birth_year");
            if (!string.IsNullOrEmpty(birthDay) && !string.IsNullOrEmpty(birthMonth) && !string.IsNullOrEmpty(birthYear))
            {
                context.Identity.TryAddClaim(new Claim(ClaimTypes.DateOfBirth, $"{birthMonth:D2}/{birthDay:D2}/{birthYear:D2}",ClaimValueTypes.DateTime));
            }
            return context;
        }

        public static async Task<JObject> GetMicrosoftUser(string accessToken)
        {
            var userInformationEndpoint = "https://apis.live.net/v5.0/me";
            var request = new HttpRequestMessage(HttpMethod.Get, userInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await (new HttpClient()).SendAsync(request);
            response.EnsureSuccessStatusCode();
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Gets the Microsoft Account user ID.
        /// </summary>
        public static string GetId(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("id");
        }

        /// <summary>
        /// Gets the user's name.
        /// </summary>
        public static string GetName(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("name");
        }

        /// <summary>
        /// Gets the user's first name.
        /// </summary>
        public static string GetFirstName(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("first_name");
        }

        /// <summary>
        /// Gets the user's last name.
        /// </summary>
        public static string GetLastName(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("last_name");
        }

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        public static string GetEmail(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<JObject>("emails")?.Value<string>("preferred");
        }


    }
}