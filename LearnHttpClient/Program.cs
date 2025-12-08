using Data.CountriesAndCities;
using Services;

namespace LearnHttpClient;

class Program
{
    public static HttpClient client = new HttpClient();

    static void Main(string[] args)
    {
        string menu;

        try
        {
            CountriesAndCitiesService.LoadCities(ref client, ProgramConsts.BaseUrlForCities);
        }
        catch (System.Net.Http.HttpRequestException)
        {
            Console.WriteLine("No internet. Check internet connection.");
        }

        do
        {
            try
            {
                CityResult? city = CountriesAndCitiesService.GetCity();

                if (city != null && !string.IsNullOrWhiteSpace(city.City))
                {
                    string weather = WeatherService.GetWeather(ref client, ProgramConsts.BaseUrlForWeather, ProgramConsts.ApiKey, city);
                    Console.WriteLine($"The weather in {city} is:\n{weather}");
                }
            }
            catch (System.Net.Http.HttpRequestException)
            {
                Console.WriteLine("No internet. Check internet connection.");
            }
            catch
            {
                Console.WriteLine("log: error.");
            }

            Console.Write("Press enter to continue or type 'exit' to exit: ");
            menu = Console.ReadLine() ?? "";
        } while (!menu.Contains("exit"));

        Console.WriteLine("log: exit...");
    }
}