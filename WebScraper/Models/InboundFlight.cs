namespace WebScraper.Models
{
    public class InboundFlight
    {
        public int RecommendationId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Taxes { get; set; }
        public List<Flight> InboundFlights { get; set; }
    }
}
