using System;
using System.Net;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace LearnHttpClient;

class Program
{
    public static HttpClient client = new HttpClient();

    public static string baseUrlForCities = "https://countriesnow.space/api/v0.1";

    public static string baseUrlForWeather = "https://api.openweathermap.org/data/2.5";

    public static string apiKey = "b11365829b240e9f34947ab9187f258b";

    public static CountriesAndCities? countriesAndCitiesData;

    public static WeatherData? weather;

    static void Main(string[] args)
    {
        string menu; ;
        LoadCities();

        do
        {
            try
            {
                CityResult? city = GetCity();

                if (city != null && !string.IsNullOrWhiteSpace(city.City))
                {
                    string weather = GetWeather(city);
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

    public static async Task LoadCities()
    {
        bool loading = true;

        // Thread with loading animation
        var spinner = Task.Run(async () =>
        {
            string[] dots = { "", ".", "..", "..." };
            int i = 0;

            while (loading)
            {
                Console.Clear();
                PrintL($"cities loading{dots[i]}");
                i = (i + 1) % dots.Length;
                await Task.Delay(300);
            }
        });

        // Cities loading
        var endpoint = new Uri(baseUrlForCities + "/countries");
        var result = client.GetAsync(endpoint).Result;
        var json = result.Content.ReadAsStringAsync().Result;
        countriesAndCitiesData = JsonConvert.DeserializeObject<CountriesAndCities>(json);

        // When the load is copmleted
        loading = false;
        Console.Clear();
        await spinner;

        if (countriesAndCitiesData == null || countriesAndCitiesData.Data == null)
        {
            PrintL("log: JSON parse error.");
            PrintL("log: cities aren't loaded.");
        }

        return;
    }

    public static CityResult? GetCity()
    {
        string input;

        do
        {
            Console.Write("Enter city name (at least three symbols) or type 'exit' to exit: ");
            input = Console.ReadLine()?.Trim() ?? "";

            if (input.Contains("exit"))
            {
                return null;
            }
            else if (string.IsNullOrWhiteSpace(input) || input.Length < 3)
            {
                PrintL("Input is too short or empty. Try again...");
                continue;
            }

            return FindExistingCity(input);
        } while (true);
    }

    public static CityResult? FindExistingCity(string input)
    {
        if (countriesAndCitiesData == null)
        {
            return new CityResult(input, null);
        }

        string search = input.ToLower();

        var matchedCities = countriesAndCitiesData.Data
            .SelectMany(country => country.Cities
                .Where(city => city.ToLower().Contains(search))
                .Select(city => new CityResult(city, country.Iso2))
            )
            //.Distinct()
            //.OrderBy(c => c)
            .OrderBy(x => x.City)
            .ToList();

        if (matchedCities.Count == 0)
        {
            PrintL("No cities found.");
            return null;
        }
        else if (matchedCities.Count == 1)
        {
            return matchedCities[0];
        }
        else
        {
            int cityNumber;

            do
            {
                PrintL("City not found. Perhaps you mean the following:");
                PrintL("Matched cities:");
                for (int i = 0; i < matchedCities.Count; i++)
                {
                    PrintL($"{i}. {matchedCities[i]};");
                }
                Print("Select number from the list (or type e to exit):");

                cityNumber = int.TryParse(Console.ReadLine(), out cityNumber) ? cityNumber : -1;

                if (cityNumber == -1)
                {
                    PrintL("log: exit...");
                    return null;
                }
                else if (cityNumber >= matchedCities.Count)
                {
                    PrintL("Wron input. Try again...");
                    continue;
                }

                return matchedCities[cityNumber];
            } while (true);
        }
    }

    public static string GenerateWeatherEndpoint(CityResult city)
    {
        return $"{baseUrlForWeather}/weather?q={city}&units=metric&appid={apiKey}";
    }

    public static string GetWeather(CityResult city)
    {
        PrintL("log: loading weather...");

        var result = client.GetAsync(GenerateWeatherEndpoint(city)).Result;
        var json = result.Content.ReadAsStringAsync().Result;

        weather = JsonConvert.DeserializeObject<WeatherData>(json);

        if (weather == null || weather.Main == null)
        {
            PrintL($"log: JSON parse error.");
            PrintL($"log: {weather.Message}.");
        }
        else
        {
            PrintL("log: weather is loaded.");
        }

        if (weather.Main == null)
        {
            return "No data";
        }

        Main mainWeatherData = weather.Main;

        return
            $"Temperature: {mainWeatherData.Temp}\n" +
            $"Min temp   : {mainWeatherData.Temp_min}\n" +
            $"Max temp   : {mainWeatherData.Temp_max}\n" +
            $"Pressure   : {mainWeatherData.Pressure}\n" +
            $"Humidity   : {mainWeatherData.Humidity}";
    }
}