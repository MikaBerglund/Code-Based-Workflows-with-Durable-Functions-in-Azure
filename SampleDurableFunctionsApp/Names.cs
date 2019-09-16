using System;
using System.Collections.Generic;
using System.Text;

namespace SampleDurableFunctionsApp
{
    public static class Names
    {

        /// <summary>
        /// A HTTP (GET) triggered function that initializes a new crawl job. The URI to crawl must be specified in the <c>uri</c> query string parameter.
        /// </summary>
        public const string StartCrawlingHttp = nameof(StartCrawlingHttp);

        /// <summary>
        /// The main orchestration function that starts a crawling job. The input to the function must be a <see cref="Uri"/> that specifies the web page to crawl.
        /// </summary>
        public const string CrawlingMain = nameof(CrawlingMain);

        /// <summary>
        /// The activity function that downloads the HTML from the given URL. The input to the function must be a <see cref="Uri"/> to download from.
        /// </summary>
        public const string DownloadHtml = nameof(DownloadHtml);

    }
}
