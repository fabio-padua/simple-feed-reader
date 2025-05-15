using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SimpleFeedReader.Services;
using SimpleFeedReader.ViewModels;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SimpleFeedReader.Pages
{
    public class IndexModel : PageModel
    {
        private readonly NewsService _newsService;
        private readonly IConfiguration _configuration;  
   
        public IndexModel(NewsService newsService, IConfiguration configuration)
        {
            _newsService = newsService;
            _configuration = configuration;
        }

        public string ErrorText { get; private set; }

        public List<NewsStoryViewModel> NewsItems { get; private set; }
        public List<StockValueViewModel> StockItems { get; private set; }

        /// <summary>
        /// Simulates a CPU load by performing a CPU-intensive task for a specified duration.
        /// </summary>
        /// <remarks>
        /// This method runs a loop for 10 seconds, during which it performs a computationally 
        /// intensive operation (calculating the square root of numbers) to simulate CPU load.
        /// </remarks>
        private void SimulateCpuLoad()
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < 10000) // Run for 10 seconds
            {
                // Perform a CPU-intensive task
                for (int i = 0; i < 1000000; i++)
                {
                    double result = Math.Sqrt(i);
                }
            }
        }

        public async Task OnGet()
        {
            ViewData["Header"] = _configuration.GetValue<string>("UI:Index:Header");

            try {
                StockItems = await _newsService.GetStock();
                SimulateCpuLoad();
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging framework)
                Console.WriteLine($"An error occurred: {ex.Message}");

            }

            string feedUrl = Request.Query["feedurl"];

            if (!string.IsNullOrEmpty(feedUrl))
            {
                try
                {
                    NewsItems = await _newsService.GetNews(feedUrl);
                }
                catch (UriFormatException)
                {
                    ErrorText = "There was a problem parsing the URL.";
                    return;
                }
                catch (WebException ex) when (ex.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    ErrorText = "Unknown host name.";
                    return;
                }
                catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    ErrorText = "Syndication feed not found.";
                    return;
                }
                catch (AggregateException ae)
                {
                    ae.Handle((x) =>
                    {
                        if (x is XmlException)
                        {
                            ErrorText = "There was a problem parsing the feed. Are you sure that URL is a syndication feed?";
                            return true;
                        }
                        return false;
                    });
                }
            }
        }
    }
}
