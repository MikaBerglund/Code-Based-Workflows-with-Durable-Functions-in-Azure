using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace SampleDurableFunctionsApp
{
    public class CrawlingFunctions
    {

        [FunctionName(nameof(CrawlingMain))]
        public async Task<IEnumerable<string>> CrawlingMain([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {
            var uri = context.GetInput<Uri>();

            // We use an activity function to download HTML from the URI specified as input. We use an extension method that incorporates
            // a default retry logic, because we can't be sure that the resource is aways available.
            var html = await context.CallActivityWithDefaultRetryAsync<string>(nameof(DownloadHtml), uri);

            // Links are parsed in another activity function. This is done without retry logic, because the activity function does not rely
            // on external resources. This also shows how you can call activity functions without retry logic.
            var links = await context.CallActivityAsync<IEnumerable<string>>(nameof(ParseLinks), html);

            return links;
        }

        [FunctionName(nameof(IndexHtml))]
        public async Task IndexHtml([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {
            // This method just demonstrates how you can start new orchestrations even from activity functions. This
            // orchestration is triggered from the DownloadHtmlAsync function below.
            var input = context.GetInput<Tuple<Uri, string>>();
            var uri = input.Item1;
            var html = input.Item2;

        }

        [FunctionName(nameof(DownloadHtml))]
        public async Task<string> DownloadHtml([ActivityTrigger] IDurableOrchestrationContext context, [DurableClient] IDurableOrchestrationClient orchestrationClient, ILogger logger)
        {
            var uri = context.GetInput<Uri>();
            var client = new HttpClient();
            var html = await client.GetStringAsync(uri);

            // This just shows you how you could fire up new orchestrations even from an activity function. You just add a DurableOrchestrationClient
            // parameter to the signature with the OrchestrationClient attribute.
            await orchestrationClient.StartNewAsync(nameof(IndexHtml), new Tuple<Uri, string>(uri, html));

            return html;
        }

        [FunctionName(nameof(ParseLinks))]
        public async Task<IEnumerable<string>> ParseLinks([ActivityTrigger] IDurableActivityContext context, ILogger logger)
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
