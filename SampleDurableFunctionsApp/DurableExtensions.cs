using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SampleDurableFunctionsApp
{
    /// <summary>
    /// A class that defines extensions to be used by orchestration and activity functions.
    /// </summary>
    public static class DurableExtensions
    {

        /// <summary>
        /// The default retry options to use.
        /// </summary>
        private static RetryOptions defaultRetryOptions = new RetryOptions(TimeSpan.FromMinutes(1), 10);

        public static async Task CallActivityWithDefaultRetryAsync(this IDurableOrchestrationContext context, string functionName, object input = null)
        {
            await context.CallActivityWithRetryAsync(functionName, defaultRetryOptions, input);
        }

        public static async Task<TResult> CallActivityWithDefaultRetryAsync<TResult>(this IDurableOrchestrationContext context, string functionName, object input = null)
        {
            return await context.CallActivityWithRetryAsync<TResult>(functionName, defaultRetryOptions, input);
        }
    }
}
