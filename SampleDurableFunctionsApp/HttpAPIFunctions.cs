using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SampleDurableFunctionsApp
{
    public class HttpAPIFunctions
    {

        [FunctionName(nameof(StartCrawlingHttp))]
        public async Task<HttpResponseMessage> StartCrawlingHttp([HttpTrigger(authLevel: AuthorizationLevel.Function)]HttpRequestMessage request, [DurableClient] IDurableOrchestrationClient client, ILogger logger)
        {
            var url = request.RequestUri.ParseQueryString()?.GetValues("uri")?.FirstOrDefault();
            if(string.IsNullOrEmpty(url))
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    Content = new StringContent($"{{\"message\": \"No URI to crawl. The URI must be specified in the 'uri' query string parameter.\"}}", Encoding.UTF8, "application/json")
                };
            }

            var instanceId = await client.StartNewAsync(nameof(CrawlingFunctions.CrawlingMain), new Uri(url));
            return client.CreateCheckStatusResponse(request, instanceId);
        }
        
    }
}
