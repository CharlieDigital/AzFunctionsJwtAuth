using System;
using System.Collections.Generic;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Http;

namespace AzFunctionsJwtAuth
{
    /// <summary>
    ///     Wrapper class for encapsulating claims parsing.
    /// </summary>
    public class AuthenticationInfo
    {
        public bool IsValid { get; }
        public string Username { get; }
        public string Role { get; }

        public AuthenticationInfo(HttpRequest request)
        {
            // Check if we have a header.
            if (!request.Headers.ContainsKey("Authorization"))
            {
                IsValid = false;

                return;
            }

            string authorizationHeader = request.Headers["Authorization"];

            // Check if the value is empty.
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                IsValid = false;

                return;
            }

            // Check if we can decode the header.
            IDictionary<string, object> claims = null;

            try
            {
                if (authorizationHeader.StartsWith("Bearer"))
                {
                    authorizationHeader = authorizationHeader.Substring(7);
                }

                // Validate the token and decode the claims.
                claims = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret(Constants.SECRET_KEY)
                    .MustVerifySignature()
                    .Decode<IDictionary<string, object>>(authorizationHeader);
            }
            catch (Exception exception)
            {
                IsValid = false;

                return;
            }

            // Check if we have user claim.
            if (!claims.ContainsKey("username"))
            {
                IsValid = false;

                return;
            }

            IsValid = true;
            Username = Convert.ToString(claims["username"]);
            Role = Convert.ToString(claims["role"]);
        }
    }
}
