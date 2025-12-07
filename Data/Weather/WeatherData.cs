namespace Data.Weather;

public enum Units
{
    Standard,
    Metric,
    Imperial
}

public class WeatherData
{
    public Main? Main { get; set; }

    public int? Cod { get; set; }

    public string? Message { get; set; }
}
