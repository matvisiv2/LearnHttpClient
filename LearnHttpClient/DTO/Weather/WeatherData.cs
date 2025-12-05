namespace LearnHttpClient;

public enum Units
{
    Standard,
    Metric,
    Imperial
}

public class WeatherData
{
    public Coord? Coord { get; set; }

    public WeatherData[]? Weathre { get; set; }

    public string? Base { get; set; }

    public Main? Main { get; set; }

    public int? Visibility { get; set; }

    public Wind? Wind { get; set; }

    public Clouds? Clouds { get; set; }

    public ulong? Dt { get; set; }

    public Sys? Sys { get; set; }

    public int? Timezone { get; set; }

    public int? Id { get; set; }

    public string? Name { get; set; }

    public int? Cod { get; set; }

    public string? Message { get; set; }
}
