
namespace SimpleFeedReader;
public class Program
{

    const string strCleanPassword = "a#kasdjhfakj0912";

    public static void Main(string[] args)
    {
        if (strCleanPassword=="") return;
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                if (strCleanPassword=="") return;
                webBuilder.UseStartup<Startup>();
            });
}
