using WebScraper.Interfaces;
using WebScraper.Models;

namespace WebScraper.Services
{
    public class DataExtractionServices: IDataExtractionServices
    {
        public List<FlightCombination> ExtractFlightCombinations(dynamic data, int maxConnections)
        {
            List<TotalAvailabilities> availabilities = ExtractTotalAvailabilities(data.totalAvailabilities);

            List<OutboundFlight> outboundFlights = new List<OutboundFlight>();
            List<InboundFlight> inboundFlights = new List<InboundFlight>();

            foreach (dynamic journey in data.journeys)
            {
                if (journey.flights.Count <= maxConnections + 1)
                {
                    int recommendationId = journey.recommendationId;
                    decimal taxes = CalculateJourneyTaxes(journey);
                    decimal totalPrice = GetTotalPrice(availabilities, recommendationId);

                    List<Flight> journeyFlights = ExtractJourneyFlights(journey);

                    if (journey.direction == "I")
                    {
                        outboundFlights.Add(CreateOutboundFlight(recommendationId, taxes, totalPrice, journeyFlights));
                    }
                    else if (journey.direction == "V")
                    {
                        inboundFlights.Add(CreateInboundFlight(recommendationId, taxes, totalPrice, journeyFlights));
                    }
                }
            }

            List<FlightCombination> flightPairs = FindMatchingFlightCombinations(outboundFlights, inboundFlights);

            return flightPairs;
        }

        public List<FlightCombination> FindCheapestPriceCombinations(List<FlightCombination> flightCombinations)
        {
            decimal cheapestPrice = decimal.MaxValue;

            foreach (var combination in flightCombinations)
            {
                if (combination.TotalPrice < cheapestPrice)
                {
                    cheapestPrice = combination.TotalPrice;
                }
            }

            List<FlightCombination> cheapestCombinations = flightCombinations
                .Where(c => c.TotalPrice == cheapestPrice)
                .ToList();

            return cheapestCombinations;
        }

        private List<TotalAvailabilities> ExtractTotalAvailabilities(dynamic totalAvailabilities)
        {
            List<TotalAvailabilities> availabilities = new List<TotalAvailabilities>();

            foreach (dynamic availability in totalAvailabilities)
            {
                availabilities.Add(new TotalAvailabilities()
                {
                    Id = availability.recommendationId,
                    Total = availability.total
                });
            }

            return availabilities;
        }

        private decimal CalculateJourneyTaxes(dynamic journey)
        {
            return journey.importTaxAdl + journey.importTaxChd + journey.importTaxInf;
        }

        private decimal GetTotalPrice(List<TotalAvailabilities> availabilities, int recommendationId)
        {
            TotalAvailabilities availability = availabilities.FirstOrDefault(a => a.Id == recommendationId);
            return availability?.Total ?? 0;
        }

        private List<Flight> ExtractJourneyFlights(dynamic journey)
        {
            List<Flight> journeyFlights = new List<Flight>();

            foreach (dynamic flight in journey.flights)
            {
                Flight extractedFlight = new Flight()
                {
                    AirportDeparture = flight.airportDeparture.code,
                    AirportArrival = flight.airportArrival.code,
                    TimeDeparture = flight.dateDeparture,
                    TimeArrival = flight.dateArrival,
                    CompanyCode = flight.companyCode,
                    FlightNumber = flight.number
                };

                journeyFlights.Add(extractedFlight);
            }

            return journeyFlights;
        }

        private OutboundFlight CreateOutboundFlight(int recommendationId, decimal taxes, decimal totalPrice, List<Flight> journeyFlights)
        {
            return new OutboundFlight()
            {
                RecommendationId = recommendationId,
                Taxes = taxes,
                TotalPrice = totalPrice,
                OutboundFlights = journeyFlights
            };
        }

        private InboundFlight CreateInboundFlight(int recommendationId, decimal taxes, decimal totalPrice, List<Flight> journeyFlights)
        {
            return new InboundFlight()
            {
                RecommendationId = recommendationId,
                Taxes = taxes,
                TotalPrice = totalPrice,
                InboundFlights = journeyFlights
            };
        }

        private List<FlightCombination> FindMatchingFlightCombinations(List<OutboundFlight> outboundFlights, List<InboundFlight> inboundFlights)
        {
            List<FlightCombination> flightPairs = new List<FlightCombination>();

            foreach (OutboundFlight outboundFlight in outboundFlights)
            {
                foreach (InboundFlight inboundFlight in inboundFlights)
                {
                    if (outboundFlight.RecommendationId == inboundFlight.RecommendationId)
                    {
                        flightPairs.Add(new FlightCombination()
                        {
                            TotalPrice = outboundFlight.TotalPrice,
                            TotalTaxes = inboundFlight.Taxes + outboundFlight.Taxes,
                            OutboundFlight = outboundFlight,
                            InboundFlight = inboundFlight
                        });
                    }
                }
            }

            return flightPairs;
        }
    }
}
