namespace WebScraper.Models
{
    public class FlightCombination
    {
        public decimal TotalPrice { get; set; }
        public decimal TotalTaxes { get; set; }
        public OutboundFlight OutboundFlight { get; set; }
        public InboundFlight InboundFlight { get; set; }
    }
}
