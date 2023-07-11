using WebScraper.Models;

namespace WebScraper.Interfaces
{
    public interface IDataExtractionServices
    {
        List<FlightCombination> ExtractFlightCombinations(dynamic data, int maxConnections);
        List<FlightCombination> FindCheapestPriceCombinations(List<FlightCombination> flightCombinations);
    }
}
