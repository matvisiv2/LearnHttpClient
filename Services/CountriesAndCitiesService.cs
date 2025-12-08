using Newtonsoft.Json;
using Data.CountriesAndCities;

namespace Services;

public class CountriesAndCitiesService
{
    public static CountriesAndCitiesData? countriesAndCitiesData;

    public static void LoadCities(ref HttpClient client, string BaseUrlForCities)
    {
        try
        {
            Console.WriteLine($"Cities is loading...");

            string endpoint = BaseUrlForCities + "/countries";
            var result = client.GetAsync(endpoint).GetAwaiter().GetResult();
            var json = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            countriesAndCitiesData = JsonConvert.DeserializeObject<CountriesAndCitiesData>(json);

            Console.Clear();

            if (countriesAndCitiesData == null || countriesAndCitiesData.Data == null)
            {
                Console.WriteLine("log: JSON parse error.");
                Console.WriteLine("log: cities aren't loaded.");
            }
        }
        catch (System.Net.Http.HttpRequestException)
        {
            Console.WriteLine("Log: no internet. Check internet connection.");
        }
        catch
        {
            Console.WriteLine("Log: error while loading cities.");
        }
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
                Console.WriteLine("Input is too short or empty. Try again...");
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
            .OrderBy(x => x.City)
            .ToList();

        if (matchedCities.Count == 0)
        {
            Console.WriteLine("No cities found.");
            return null;
        }
        else if (matchedCities.Count == 1)
        {
            return matchedCities.FirstOrDefault();
        }
        else
        {
            int cityNumber;

            do
            {
                Console.WriteLine("City not found. Perhaps you mean the following:");
                Console.WriteLine("Matched cities:");
                for (int i = 0; i < matchedCities.Count; i++)
                {
                    Console.WriteLine($"{i}. {matchedCities[i]};");
                }
                Console.Write("Select number from the list (or type e to exit):");

                string inputCityNumber = Console.ReadLine() ?? "";

                if (inputCityNumber.Contains("exit"))
                {
                    Console.WriteLine("log: exit...");
                    return null;
                }

                cityNumber = int.TryParse(inputCityNumber, out cityNumber) ? cityNumber : -1;

                if (cityNumber >= matchedCities.Count || cityNumber < 0)
                {
                    Console.WriteLine("Wron input. Try again...");
                    continue;
                }

                return matchedCities[cityNumber];
            } while (true);
        }
    }
}
