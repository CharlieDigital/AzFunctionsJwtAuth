using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzFunctionsJwtAuth
{
    /// <summary>
    ///     Just a sample service which is fully authorized.
    /// </summary>
    public class SampleAuthorizedService : AuthorizedServiceBase
    {
        [FunctionName("AuthorizedEcho")]
        public async Task<IActionResult> AuthorizedEcho(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "authorizedecho/{message}")]
            HttpRequest req,
            string message,
            ILogger log)
        {
            return new OkObjectResult($"{Auth.Username} sent message {message}");
        }
    }
}
