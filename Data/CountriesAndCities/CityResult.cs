namespace Data.CountriesAndCities
{
    public class CityResult(string? city, string? country)
    {
        public string? City { get; set; } = city;

        public string? Country { get; set; } = country;

        public override string ToString()
        {
            return $"{City},{Country}";
        }
    }
}
