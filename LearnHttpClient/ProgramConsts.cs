using Microsoft.Extensions.Configuration;

namespace LearnHttpClient;

public class ProgramConsts
{
    public static string BaseUrlForCities;

    public static string BaseUrlForWeather;

    public static string ApiKey;

    public static int TimeoutSec = 10;

    public static void LoadConfiguration()
    {
        try
        {
            Console.WriteLine("Loading configuration...");

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            BaseUrlForCities = config["Api:BaseUrlForCities"]!;
            BaseUrlForWeather = config["Api:BaseUrlForWeather"]!;
            ApiKey = config["Api:ApiKey"]!;
            TimeoutSec = int.Parse(config["HttpClient:TimeoutSeconds"]!);
        }
        catch (System.IO.FileNotFoundException)
        {
            Console.WriteLine("Configuration file not found.");
        }
    }
}
