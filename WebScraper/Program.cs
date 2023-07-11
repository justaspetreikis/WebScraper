using WebScraper.Interfaces;
using WebScraper.Models;
using WebScraper.Services;

string baseUrl = "http://homeworktask.infare.lt/";
int maxConnections = 1;

List<SearchParameters> searchParameters = new List<SearchParameters>
        {
            new SearchParameters { From = "JFK", To = "CPH", Depart = "2023-08-10", Return = "2023-08-16" },
            new SearchParameters { From = "MAD", To = "AUH", Depart = "2023-08-09", Return = "2023-08-16" },
            new SearchParameters { From = "MAD", To = "FUE", Depart = "2023-08-10", Return = "2023-08-16" },
            new SearchParameters { From = "JFK", To = "CPH", Depart = "2023-08-10", Return = "2023-08-16" },
            new SearchParameters { From = "MAD", To = "FUE", Depart = "2023-08-09", Return = "2023-08-20" },
            new SearchParameters { From = "MAD", To = "AUH", Depart = "2023-08-09", Return = "2023-08-20" },
            new SearchParameters { From = "CPH", To = "MAD", Depart = "2023-08-09", Return = "2023-08-16" },
            new SearchParameters { From = "MAD", To = "AUH", Depart = "2023-08-09", Return = "2023-08-16" },
            new SearchParameters { From = "JFK", To = "AUH", Depart = "2023-08-09", Return = "2023-08-16" },
            new SearchParameters { From = "MAD", To = "AUH", Depart = "2023-08-15", Return = "2023-08-16" },
        };

using (HttpClient client = new HttpClient())
{
    IDataExtractionServices scraper = new DataExtractionServices();
    IDataWriterServices dataWriter = new DataWriterServices();

    // Delete the files if they exist before running the program
    string cheapestFilePath = "cheapestFlightData.csv";
    string allFlightsFilePath = "allflightsData.csv";
    
    dataWriter.DeleteExistingFiles(cheapestFilePath, allFlightsFilePath);

    foreach (var parameters in searchParameters)
    {
        string jsonResponse = "";

        try
        {
            // Create the URL with the search parameters
            string url = $"{baseUrl}search.php?from={parameters.From}&to={parameters.To}&depart={parameters.Depart}&return={parameters.Return}";

            // Send a GET request to the URL
            HttpResponseMessage response = await client.GetAsync(url);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as JSON
                jsonResponse = await response.Content.ReadAsStringAsync();

                // Parse the JSON response
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);

                // Extract and process the flight combinations
                List<FlightCombination> flightCombinations = scraper.ExtractFlightCombinations(data.body.data, maxConnections);

                // Find the cheapest price combinations
                var cheapestCombinations = scraper.FindCheapestPriceCombinations(flightCombinations);

                // Save the extracted data into a CSV file
                dataWriter.SaveToCSV(cheapestCombinations, cheapestFilePath);
                dataWriter.SaveToCSV(flightCombinations, allFlightsFilePath);
            }
            else
            {
                Console.WriteLine("Error occurred: " + response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + jsonResponse);
        }
    }
}

Console.WriteLine("Data extraction completed.");
Console.ReadLine();