using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Insights
{
    public class CabDemandAnalysisMenuAction: IMenuAction
    {
        private readonly IDataService _dataService;
        private readonly IAppLogger _appLogger;

        public CabDemandAnalysisMenuAction(IDataService dataService, IAppLogger appLogger)
        {
            _dataService = dataService;
            _appLogger = appLogger;
        }

        public string Title => "Cab Demand Analysis";
        public string Description => "Find cities with high cab demand and peak demand times";
        public char KeyChar => '3';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== Cab Demand Analysis ===");
                Console.WriteLine();

                // Get all trips and locations
                var allTrips = await _dataService.GetAllTripsAsync();
                var allLocations = await _dataService.GetAllLocationsAsync();
                var locationDict = allLocations.ToDictionary(l => l.Id, l => l);

                // Filter completed trips
                var completedTrips = allTrips.Where(t => 
                    t.TripStatus == TripStatus.COMPLETED &&
                    t.StartTime.HasValue)
                    .ToList();

                if (completedTrips.Count == 0)
                {
                    Console.WriteLine("No completed trips found for analysis.");
                    return false;
                }

                Console.WriteLine($"Analyzing demand from {completedTrips.Count} completed trips...");
                Console.WriteLine();

                // 1. City-wise demand analysis
                Console.WriteLine("1. CITY-WISE DEMAND ANALYSIS");
                Console.WriteLine("=============================");

                var cityDemand = new Dictionary<string, int>();
                var cityDepartures = new Dictionary<string, int>();
                var cityArrivals = new Dictionary<string, int>();

                foreach (var trip in completedTrips)
                {
                    // Count departures (from location)
                    if (locationDict.ContainsKey(trip.FromLocation.Id))
                    {
                        var fromCity = locationDict[trip.FromLocation.Id].City;
                        if (cityDepartures.ContainsKey(fromCity))
                            cityDepartures[fromCity]++;
                        else
                            cityDepartures[fromCity] = 1;
                    }

                    // Count arrivals (to location)
                    if (locationDict.ContainsKey(trip.ToLocation.Id))
                    {
                        var toCity = locationDict[trip.ToLocation.Id].City;
                        if (cityArrivals.ContainsKey(toCity))
                            cityArrivals[toCity]++;
                        else
                            cityArrivals[toCity] = 1;
                    }
                }

                // Combine departures and arrivals for total demand
                var allCities = cityDepartures.Keys.Union(cityArrivals.Keys).Distinct();
                foreach (var city in allCities)
                {
                    var departures = cityDepartures.ContainsKey(city) ? cityDepartures[city] : 0;
                    var arrivals = cityArrivals.ContainsKey(city) ? cityArrivals[city] : 0;
                    cityDemand[city] = departures + arrivals;
                }

                // Top cities by demand
                var topCities = cityDemand.OrderByDescending(x => x.Value).Take(10);
                Console.WriteLine("Top 10 Cities by Total Cab Demand:");
                Console.WriteLine($"{"Rank",-5} {"City",-20} {"Total Trips",-12} {"Departures",-12} {"Arrivals",-12}");
                Console.WriteLine(new string('-', 70));

                int rank = 1;
                foreach (var city in topCities)
                {
                    var departures = cityDepartures.ContainsKey(city.Key) ? cityDepartures[city.Key] : 0;
                    var arrivals = cityArrivals.ContainsKey(city.Key) ? cityArrivals[city.Key] : 0;
                    Console.WriteLine($"{rank,-5} {city.Key,-20} {city.Value,-12} {departures,-12} {arrivals,-12}");
                    rank++;
                }

                Console.WriteLine();
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in CabDemandAnalysisMenuAction", ex);
                Console.WriteLine("An error occurred while analyzing cab demand patterns.");
                return false;
            }
        }
    }
}
