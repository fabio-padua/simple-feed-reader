
namespace SimpleFeedReader;
public class Program
{

    const string someString = "a#kasdjhfakj0912";

    public static void Main(string[] args)
    {
        if (someString=="") return;
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                if (someString=="") return;
                webBuilder.UseStartup<Startup>();
            });
}
