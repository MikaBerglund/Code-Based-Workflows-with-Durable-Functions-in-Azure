using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SampleDurableFunctionsApp
{
    public class CrawlingFunctions
    {

        [FunctionName(Names.CrawlingMain)]
        public async Task<IEnumerable<string>> CrawlingMainAsync([OrchestrationTrigger]DurableOrchestrationContext context, ILogger logger)
        {
            var uri = context.GetInput<Uri>();

            // We use an activity function to download HTML from the URI specified as input. We use an extension method that incorporates
            // a default retry logic, because we can't be sure that the resource is aways available.
            var html = await context.CallActivityWithDefaultRetryAsync<string>(Names.DownloadHtml, uri);

            // Links are parsed in another activity function. This is done without retry logic, because the activity function does not rely
            // on external resources. This also shows how you can call activity functions without retry logic.
            var links = await context.CallActivityAsync<IEnumerable<string>>(Names.ParseLinks, html);

            return links;
        }

        [FunctionName(Names.DownloadHtml)]
        public async Task<string> DownloadHtmlAsync([ActivityTrigger]DurableActivityContext context, ILogger logger)
        {
            var uri = context.GetInput<Uri>();
            var client = new HttpClient();
            var html = await client.GetStringAsync(uri);
            return html;
        }

        [FunctionName(Names.ParseLinks)]
        public async Task<IEnumerable<string>> ParseLinksAsync([ActivityTrigger]DurableActivityContext context, ILogger logger)
        {
            var html = context.GetInput<string>();
            var list = new List<string>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            foreach(var ahref in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                var href = ahref.GetAttributeValue("href", null);
                if(!string.IsNullOrEmpty(href) && !list.Contains(href))
                {
                    list.Add(href);
                }
            }

            return await Task.FromResult(list);
        }
    }
}
