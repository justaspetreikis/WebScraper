using WebScraper.Models;

namespace WebScraper.Interfaces
{
    public interface IDataWriterServices
    {
        void SaveToCSV(List<FlightCombination> flightCombinations, string filePath);
        void DeleteExistingFiles(params string[] filePaths);
    }
}
