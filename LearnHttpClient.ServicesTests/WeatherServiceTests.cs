using LearnHttpClient.Data.CountriesAndCities;
using LearnHttpClient.Data.Weather;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace LearnHttpClient.Services.Tests;

[TestClass()]
public class WeatherServiceTests
{
    private HttpClient? client = null;
    private string BaseUrlForCities = string.Empty;
    private string BaseUrlForWeather = string.Empty;
    private string ApiKey = string.Empty;
    private int TimeoutSeconds = 0;

    [TestInitialize]
    public void TestInitialize()
    {
        Debug.WriteLine("Test Initialize");

        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        BaseUrlForCities = config["Api:BaseUrlForCities"]!;
        BaseUrlForWeather = config["Api:BaseUrlForWeather"]!;
        ApiKey = config["Api:ApiKey"]!;
        TimeoutSeconds = int.Parse(config["HttpClient:TimeoutSeconds"]!);

        client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);
    }

    [TestMethod()]
    public void GenerateWeatherEndpointTest()
    {
        // arrange
        CityResult? city = new CityResult("Kyiv", "UA");
        string expected = $"{BaseUrlForWeather}/weather?q={city}&units={Units.Metric.ToString().ToLower()}&appid={ApiKey}";

        // act
        string actual = WeatherService.GenerateWeatherEndpoint(BaseUrlForWeather, ApiKey, city);

        // assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void GetWeatherTest()
    {
        // arrange
        CityResult city = new CityResult("Shlyamburg", "XX");
        string expectedText = "city not found";

        // act
        string res = WeatherService.GetWeather(ref client, BaseUrlForWeather, ApiKey, city);

        // assert
        Debug.WriteLine($"res.Contains(expectedText) = {res.Contains(expectedText)}");
        Assert.IsTrue(res.Contains(expectedText));
    }
}