using System;
using System.Net;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;

namespace LearnHttpClient;

class Program
{
    public static string baseUrlForCities = "https://countriesnow.space/api/v0.1";

    public static string baseUrlForWeather = "https://api.openweathermap.org/data/2.5";

    public static string apiKey = "b11365829b240e9f34947ab9187f258b";

    static CountriesAndCities? root;

    static WeatherData? weather;

    static void Main(string[] args)
    {
        string menu; ;
        LoadCities();

        do
        {
            try
            {
                string city = GetCity();

                if (!string.IsNullOrWhiteSpace(city))
                {
                    string weather = GetWeather(city);
                    PrintL($"The weather in {city} is: {weather}");
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

    public static void LoadCities()
    {
        PrintL("log: loading cities...");

        var endpoint = new Uri(baseUrlForCities + "/countries");
        var client = new HttpClient();
        var result = client.GetAsync(endpoint).Result;
        var json = result.Content.ReadAsStringAsync().Result;
        root = JsonConvert.DeserializeObject<CountriesAndCities>(json);

        if (root == null || root.Data == null)
        {
            PrintL("log: JSON parse error.");
        }
        else
        {
            PrintL("log: cities are loaded.");
        }

        return;
    }

    public static string GetCity()
    {
        string input;

        do
        {
            Console.Write("Enter city name (at least three symbols) or type 'exit' to exit: ");
            input = Console.ReadLine()?.Trim() ?? "";

            if (input.Contains("exit"))
            {
                return string.Empty;
            }
            else if (string.IsNullOrWhiteSpace(input) || input.Length < 3)
            {
                PrintL("Input is too short or empty. Try again...");
                continue;
            }

            return FindExistingCity(input);
        } while (true);
    }

    public static string FindExistingCity(string input)
    {
        if (root == null)
        {
            return input;
        }

        string search = input.ToLower();

        var matchedCities = root.Data
            .SelectMany(country => country.Cities)
            .Where(city => city.ToLower().Contains(search))
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        if (matchedCities.Count == 0)
        {
            PrintL("No cities found.");
            return string.Empty;
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
                    return "";
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

    public static string GenerateWeatherEndpoint(string city)
    {
        return $"{baseUrlForWeather}/weather?q={city},ua&units=metric&appid={apiKey}";
    }

    public static string GetWeather(string city)
    {
        PrintL("log: loading weather...");

        var client = new HttpClient();
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

        return "";
    }
}