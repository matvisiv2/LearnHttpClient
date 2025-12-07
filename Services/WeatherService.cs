using Data.CountriesAndCities;
using Data.Weather;
using Newtonsoft.Json;

namespace Services;

public class WeatherService
{
    public static string GenerateWeatherEndpoint(string baseUrlForWeather, string apiKey, CityResult city)
    {
        return $"{baseUrlForWeather}/weather?q={city}&units={Units.Metric.ToString().ToLower()}&appid={apiKey}";
    }

    public static string GetWeather(ref HttpClient client, string baseUrlForWeather, string apiKey, CityResult city)
    {
        PrintL("log: loading weather...");

        string endpoint = GenerateWeatherEndpoint(baseUrlForWeather, apiKey, city);
        var result = client.GetAsync(endpoint).GetAwaiter().GetResult();
        var json = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        WeatherData? weather = JsonConvert.DeserializeObject<WeatherData>(json);

        if (weather == null)
        {
            PrintL($"log: JSON parse error.");
        }
        else if (weather.Main == null)
        {
            PrintL($"log: {weather.Message}.");
        }
        else
        {
            PrintL("log: weather is loaded.");
        }

        if (weather?.Main == null)
        {
            return "No data";
        }

        Main mainWeatherData = weather.Main;

        return
            $"Temperature: {mainWeatherData.Temp} C{(char)176}\n" +
            $"Min temp   : {mainWeatherData.Temp_min} C{(char)176}\n" +
            $"Max temp   : {mainWeatherData.Temp_max} C{(char)176}\n" +
            $"Pressure   : {mainWeatherData.Pressure} Pa\n" +
            $"Humidity   : {mainWeatherData.Humidity} %";
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
