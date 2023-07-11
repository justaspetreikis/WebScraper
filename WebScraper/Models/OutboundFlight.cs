namespace WebScraper.Models
{
    public class OutboundFlight
    {
        public int RecommendationId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Taxes { get; set; }
        public List<Flight> OutboundFlights { get; set; }
    }
}
