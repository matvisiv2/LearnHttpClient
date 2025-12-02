using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnHttpClient;

public class CountriesAndCities
{
    public bool? Error { get; set; }
    public string? Msg { get; set; }
    public CountryData[]? Data { get; set; }
}

public class CountryData
{
    public string? Iso2 { get; set; }

    public string? Iso3 { get; set; }

    public string? Country { get; set; }

    public string[]? Cities { get; set; }
}
public class CityResult(string? city, string? country)
{
    public string? City { get; set; } = city;

    public string? Country { get; set; } = country;

    public override string ToString()
    {
        return $"{City},{Country}";
    }
}