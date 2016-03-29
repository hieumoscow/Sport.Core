// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Facebook;
using Newtonsoft.Json.Linq;

namespace Sport.Service.Utilities.Auth
{
    public static class FacebookHelper
    {

        public static async Task<FacebookAuthenticatedContext> UpdateContext(FacebookAuthenticatedContext context)
        {
            JObject payload = context.User;

            if (context.User == null)
            {
                payload = await GetFacebookUser(context.AccessToken);
            }

            var identifier = GetId(payload);
            if (!string.IsNullOrEmpty(identifier))
            {
                context.Identity.TryAddClaim(new Claim(ClaimTypes.NameIdentifier, identifier, ClaimValueTypes.String, "Facebook"));
            }

            var userName = GetUserName(payload);
            if (!string.IsNullOrEmpty(userName))
            {
                context.Identity.TryAddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, userName, ClaimValueTypes.String));
            }

            var email = GetEmail(payload);
            if (!string.IsNullOrEmpty(email))
            {
                context.Identity.TryAddClaim(new Claim(ClaimTypes.Email, email, ClaimValueTypes.String));
            }

            var name = GetName(payload);
            if (!string.IsNullOrEmpty(name))
            {
                context.Identity.TryAddClaim(new Claim("urn:facebook:name", name, ClaimValueTypes.String));

                // Many Facebook accounts do not set the UserName field.  Fall back to the Name field instead.
                if (string.IsNullOrEmpty(userName))
                {
                    context.Identity.TryAddClaim(new Claim(context.Identity.NameClaimType, name, ClaimValueTypes.String));
                }
            }
            context.Identity.AddClaim(new Claim("urn:account:image",
                $"https://graph.facebook.com/{identifier}/picture?type=large", ClaimValueTypes.String));

            var link = GetLink(payload);
            if (!string.IsNullOrEmpty(link))
            {
                context.Identity.TryAddClaim(new Claim("urn:facebook:link", link, ClaimValueTypes.String));
            }

            var firstName = payload.Value<string>("first_name");
            if (!string.IsNullOrEmpty(firstName))
            {
                context.Identity.TryAddClaim(new Claim(ClaimTypes.GivenName, firstName, ClaimValueTypes.String));
            }

            var lastName = payload.Value<string>("last_name");
            if (!string.IsNullOrEmpty(lastName))
            {
                context.Identity.TryAddClaim(new Claim(ClaimTypes.Surname, lastName, ClaimValueTypes.String));
            }

            var gender = payload.Value<string>("gender");
            if (!string.IsNullOrEmpty(gender))
            {
                context.Identity.TryAddClaim(new Claim(ClaimTypes.Gender, gender.Equals("male").ToString(), ClaimValueTypes.Boolean));
            }

            var birthday = payload.Value<string>("birthday");
            if (!string.IsNullOrEmpty(birthday))
            {
                context.Identity.TryAddClaim(new Claim(ClaimTypes.DateOfBirth, birthday, ClaimValueTypes.DateTime));
            }
            var timezone = payload.Value<string>("timezone");
            if (!string.IsNullOrEmpty(timezone))
            {
                context.Identity.TryAddClaim(new Claim("urn:account:timezone", timezone, ClaimValueTypes.String));
            }

            return context;
        }

        public static async Task<JObject> GetFacebookUser(string accessToken)
        {
            var endpoint = QueryHelpers.AddQueryString("https://graph.facebook.com/me", "access_token", accessToken);
            var response = await (new HttpClient()).GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Gets the Facebook user ID.
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
        /// Gets the user's link.
        /// </summary>
        public static string GetLink(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("link");
        }

        /// <summary>
        /// Gets the Facebook username.
        /// </summary>
        public static string GetUserName(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("username");
        }


        /// <summary>
        /// Gets the Facebook email.
        /// </summary>
        public static string GetEmail(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("email");
        }
    }
}