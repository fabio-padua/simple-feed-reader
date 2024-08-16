using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;


namespace SimpleFeedReader.Services;
public class NewsService
{
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public NewsService(IMapper mapper, HttpClient httpClient, IConfiguration configuration)
    {
        _mapper = mapper;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    private async Task<List<StockValueViewModel>> GetStockValues(string stockKey, string apiKey)
    {
        var stockValues = new List<StockValueViewModel>();
        var apiUrl = $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={stockKey}&interval=5min&apikey={apiKey}";

        try
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var stockData = JsonSerializer.Deserialize<AlphaVantageStockData>(json);

                foreach (var item in stockData.TimeSeries)
                {
                    var stockValue = new StockValueViewModel
                    {
                        Published = DateTimeOffset.Parse(item.Key),
                        Stock = stockData.MetaData.Symbol,
                        Value = double.Parse(item.Value.Open.ToString())
                    };
                    stockValues.Add(stockValue);
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception (use a logging framework)
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return stockValues;
    }

    public async Task<List<StockValueViewModel>> GetStock()
    {
        var stock = new List<StockValueViewModel>();

        var apiKey = "24D0KAMYXBG0S3Z6";
        var stockKeyList = new List<string> { "AAPL", "MSFT", "GOOGL", "AMZN", "FB" };  

        foreach (var stockKey in stockKeyList)
        {
            var stockValues = await GetStockValues(stockKey, apiKey);
            stock.AddRange(stockValues);
        }

        return stock;
    }

    public async Task<List<NewsStoryViewModel>> GetNews(string feedUrl)
    {
        var news = new List<NewsStoryViewModel>();
        var feedUri = new Uri(feedUrl);

        using (var xmlReader = XmlReader.Create(feedUri.ToString(),
               new XmlReaderSettings { Async = true }))
        {
            try
            {
                var feedReader = new RssFeedReader(xmlReader);

                while (await feedReader.Read())
                {
                    switch (feedReader.ElementType)
                    {
                        // RSS Item
                        case SyndicationElementType.Item:
                            ISyndicationItem item = await feedReader.ReadItem();
                            var newsStory = _mapper.Map<NewsStoryViewModel>(item);
                            news.Add(newsStory);
                            break;

                        // Something else
                        default:
                            break;
                    }
                }
            }
            catch (AggregateException ae)
            {
                throw ae.Flatten();
            }
        }

        return news.OrderByDescending(story => story.Published).ToList();
    }

}

public class AlphaVantageStockData
{
    [JsonPropertyName("Meta Data")]
    public MetaData MetaData { get; set; }

    [JsonPropertyName("Time Series (5min)")]
    public Dictionary<string, TimeSeriesData> TimeSeries { get; set; }
}

public class MetaData
{
    [JsonPropertyName("1. Information")]
    public string Information { get; set; }

    [JsonPropertyName("2. Symbol")]
    public string Symbol { get; set; }

    [JsonPropertyName("3. Last Refreshed")]
    public string LastRefreshed { get; set; }

    [JsonPropertyName("4. Interval")]
    public string Interval { get; set; }

    [JsonPropertyName("5. Output Size")]
    public string OutputSize { get; set; }

    [JsonPropertyName("6. Time Zone")]
    public string TimeZone { get; set; }
}

public class TimeSeriesData
{
    [JsonPropertyName("1. open")]
    public string Open { get; set; }

    [JsonPropertyName("2. high")]
    public string High { get; set; }

    [JsonPropertyName("3. low")]
    public string Low { get; set; }

    [JsonPropertyName("4. close")]
    public string Close { get; set; }
}

public class NewsStoryProfile : Profile
{
    public NewsStoryProfile()
    {
        // Create the AutoMapper mapping profile between the 2 objects.
        // ISyndicationItem.Id maps to NewsStoryViewModel.Uri.
        CreateMap<ISyndicationItem, NewsStoryViewModel>()
            .ForMember(dest => dest.Uri, opts => opts.MapFrom(src => src.Id));
    }
}
