// See https://aka.ms/new-console-template for more information
using HeavyArrayForecast;

Console.WriteLine("Initializing...");

do
{

    Console.WriteLine("");
    Console.WriteLine("Get wheater...");
    Console.WriteLine("");

    foreach (var item in WeatherForecastHelper.GetForecasts())
    {
        Console.WriteLine(item.Date.ToString() + " " + item.TemperatureC.ToString() + " " + item.Summary.ToString());
    };

    Console.WriteLine("");
    Console.WriteLine("Sleeping for 10s...");
    Console.WriteLine("");

    Thread.Sleep(10000);

} while (true);