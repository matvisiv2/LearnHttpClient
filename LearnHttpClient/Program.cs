using Data.CountriesAndCities;
using Services;

namespace LearnHttpClient;

class Program
{
    public static HttpClient client = new HttpClient();

    static void Main(string[] args)
    {
        string menu; ;
        CountriesAndCitiesService.LoadCities(ref client, ProgramConsts.BaseUrlForCities);

        do
        {
            try
            {
                CityResult? city = CountriesAndCitiesService.GetCity();

                if (city != null && !string.IsNullOrWhiteSpace(city.City))
                {
                    string weather = WeatherService.GetWeather(ref client, ProgramConsts.BaseUrlForWeather, ProgramConsts.ApiKey, city);
                    PrintL($"The weather in {city} is:\n{weather}");
                }
            }
            catch
            {
                PrintL("log: error.");
            }

            Print("Press enter to continue or type 'exit' to exit: ");
            menu = Console.ReadLine() ?? "";
        } while (!menu.Contains("exit"));

        PrintL("log: exit...");
    }

    public static void Print(params object[] values)
    {
        Console.Write(string.Join("", values));
    }

    public static void PrintL(params object[] values)
    {
        Console.WriteLine(string.Join("", values));
    }
}