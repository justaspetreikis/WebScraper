using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using WebScraper.Interfaces;
using WebScraper.Models;

namespace WebScraper.Services
{
    public class DataWriterServices: IDataWriterServices
    {
        private readonly CsvConfiguration csvConfiguration;

        public DataWriterServices()
        {
            csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            csvConfiguration.TrimOptions = TrimOptions.Trim;
        }

        public void SaveToCSV(List<FlightCombination> flightCombinations, string filePath)
        {
            using (var writer = new StreamWriter(filePath, true))
            using (var csv = new CsvWriter(writer, csvConfiguration))
            {
                foreach (var combination in flightCombinations)
                {
                    csv.WriteField(combination.TotalPrice);
                    csv.WriteField(combination.TotalTaxes);

                    WriteFlightList(csv, combination.OutboundFlight.OutboundFlights);
                    WriteFlightList(csv, combination.InboundFlight.InboundFlights);

                    csv.NextRecord();
                }
            }
        }

        public void DeleteExistingFiles(params string[] filePaths)
        {
            foreach (var filePath in filePaths)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        private void WriteFlightList(CsvWriter csv, List<Flight> flights)
        {
            foreach (var flight in flights)
            {
                csv.WriteField(flight.AirportDeparture);
                csv.WriteField(flight.AirportArrival);
                csv.WriteField(flight.TimeDeparture.ToString("yyyy-MM-dd H:mm", CultureInfo.InvariantCulture));
                csv.WriteField(flight.TimeArrival.ToString("yyyy-MM-dd H:mm", CultureInfo.InvariantCulture));
                csv.WriteField($"{flight.CompanyCode}{flight.FlightNumber}");
            }
        }
    }
}
