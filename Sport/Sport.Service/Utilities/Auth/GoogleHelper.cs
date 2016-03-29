// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Google;
using Newtonsoft.Json.Linq;

namespace Sport.Service.Utilities.Auth
{
    /// <summary>
    /// Contains static methods that allow to extract user's information from a <see cref="JObject"/>
    /// instance retrieved from Google after a successful authentication process.
    /// </summary>
    public static class GoogleHelper
    {

        public static async Task<GoogleOAuth2AuthenticatedContext> UpdateContext(GoogleOAuth2AuthenticatedContext context)
        {
            JObject payload = context.User;

            if (payload == null)
            {
                payload = await GetGoogleUser(context.AccessToken);
            }

            var identifier = GetId(payload);

            if (!string.IsNullOrEmpty(identifier))
            {
                context.Identity.TryAddClaim(new Claim(ClaimTypes.NameIdentifier, identifier,
                    ClaimValueTypes.String, "Google"));
                context.Identity.TryAddClaim(new Claim("urn:google:id", identifier, ClaimValueTypes.String));
            }

            var name = GetName(payload);
            if (!string.IsNullOrEmpty(name))
            {
                context.Identity.TryAddClaim(new Claim(ClaimTypes.Name, name, ClaimValueTypes.String));
                context.Identity.TryAddClaim(new Claim("urn:google:name", name, ClaimValueTypes.String));
            }

            var email = GetEmail(payload);
            if (!string.IsNullOrEmpty(email))
            {
                context.Identity.TryAddClaim(new Claim(ClaimTypes.Email, email, ClaimValueTypes.String));
            }

            var firstName = GetGivenName(payload);
            if (!string.IsNullOrEmpty(firstName))
            {
                context.Identity.TryAddClaim(new Claim(ClaimTypes.GivenName, firstName, ClaimValueTypes.String));
            }

            var lastName = GetFamilyName(payload);
            if (!string.IsNullOrEmpty(lastName))
            {
                context.Identity.TryAddClaim(new Claim(ClaimTypes.Surname, lastName, ClaimValueTypes.String));
            }
            var pic = GetPicture(payload);
            if (!string.IsNullOrEmpty(pic))
            {
                context.Identity.TryAddClaim(new Claim("urn:account:image", pic, ClaimValueTypes.String));
            }
            return context;
        }

        public static async Task<JObject> GetGoogleUser(string accessToken)
        {
            var userInformationEndpoint = string.Format("https://www.googleapis.com/plus/v1/people/me");

            var request = new HttpRequestMessage(HttpMethod.Get, userInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await (new HttpClient()).SendAsync(request);
            response.EnsureSuccessStatusCode();

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
            return payload;
        }


        /// <summary>
        /// Gets the Google user ID.
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

            return user.Value<string>("displayName");
        }

        /// <summary>
        /// Gets the user's given name.
        /// </summary>
        public static string GetGivenName(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return TryGetValue(user, "name", "givenName");
        }
        /// <summary>
        /// Gets the user's family name.
        /// </summary>
        public static string GetPicture(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("picture");
        }
        /// <summary>
        /// Gets the user's family name.
        /// </summary>
        public static string GetFamilyName(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return TryGetValue(user, "name", "familyName");
        }

        /// <summary>
        /// Gets the user's profile link.
        /// </summary>
        public static string GetProfile(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("url");
        }

        /// <summary>
        /// Gets the user's email.
        /// </summary>
        public static string GetEmail(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return TryGetFirstValue(user, "emails", "value");
        }

        // Get the given subProperty from a property.
        private static string TryGetValue(JObject user, string propertyName, string subProperty)
        {
            JToken value;
            if (user.TryGetValue(propertyName, out value))
            {
                var subObject = JObject.Parse(value.ToString());
                if (subObject != null && subObject.TryGetValue(subProperty, out value))
                {
                    return value.ToString();
                }
            }
            return null;
        }

        // Get the given subProperty from a list property.
        private static string TryGetFirstValue(JObject user, string propertyName, string subProperty)
        {
            JToken value;
            if (user.TryGetValue(propertyName, out value))
            {
                var array = JArray.Parse(value.ToString());
                if (array != null && array.Count > 0)
                {
                    var subObject = JObject.Parse(array.First.ToString());
                    if (subObject != null)
                    {
                        if (subObject.TryGetValue(subProperty, out value))
                        {
                            return value.ToString();
                        }
                    }
                }
            }
            return null;
        }
    }
}
