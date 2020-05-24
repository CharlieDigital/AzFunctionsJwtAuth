using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzFunctionsJwtAuth
{
    /// <summary>
    ///     Service class for performing authentication.
    /// </summary>
    public class AuthenticationService
    {
        private readonly TokenIssuer _tokenIssuer;

        /// <summary>
        ///     Injection constructor.
        /// </summary>
        /// <param name="tokenIssuer">DI injected token issuer singleton.</param>
        public AuthenticationService(TokenIssuer tokenIssuer)
        {
            _tokenIssuer = tokenIssuer;
        }

        [FunctionName("Authenticate")]
        public async Task<IActionResult> Authenticate(
            // https://stackoverflow.com/a/52748884/116051
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth")] 
            Credentials credentials,
            ILogger log)
        {
            // Perform custom authentication here
            bool authenticated = credentials?.User.Equals("charles", StringComparison.InvariantCultureIgnoreCase) ?? false;

            if (!authenticated)
            {
                return new UnauthorizedResult();
            }

            return new OkObjectResult(_tokenIssuer.IssueTokenForUser(credentials));
        }

        [FunctionName("ChangePassword")]
        public async Task<IActionResult> ChangePassword(
            // https://stackoverflow.com/a/52748884/116051
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "changepassword")]
            HttpRequest req, // Note: we need the underlying request to get the header
            ILogger log)
        {
            // Check if we have authentication info.
            AuthenticationInfo auth = new AuthenticationInfo(req);

            if (!auth.IsValid)
            {
                return new UnauthorizedResult(); // No authentication info.
            }

            string newPassword = await req.ReadAsStringAsync();

            return new OkObjectResult($"{auth.Username} changed password to {newPassword}");
        }
    }

    /// <summary>
    ///     DTO for transferring the auth info.
    /// </summary>
    public class Credentials
    {
        public string User { get; set; }

        public string Password { get; set; }
    }
}
