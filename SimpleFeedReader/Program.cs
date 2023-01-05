
namespace SimpleFeedReader;
public class Program
{

    const string password = "a#kasdjhfakj0912";

    public static void Main(string[] args)
    {
        if (password=="") return;
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                if (password=="") return;
                webBuilder.UseStartup<Startup>();
            });
}
