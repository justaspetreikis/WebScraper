namespace WebScraper.Models
{
    public class Flight
    {
        public string AirportDeparture { get; set; }
        public string AirportArrival { get; set; }
        public DateTime TimeDeparture { get; set; }
        public DateTime TimeArrival { get; set; }
        public string CompanyCode { get; set; }
        public string FlightNumber { get; set; }
    }
}
