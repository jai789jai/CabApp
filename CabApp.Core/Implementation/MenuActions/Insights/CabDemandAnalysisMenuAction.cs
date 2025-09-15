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

                // 2. Time-based demand analysis
                Console.WriteLine("2. TIME-BASED DEMAND ANALYSIS");
                Console.WriteLine("==============================");

                // Hourly demand analysis
                var hourlyDemand = new Dictionary<int, int>();
                var dailyDemand = new Dictionary<DayOfWeek, int>();
                var monthlyDemand = new Dictionary<int, int>();

                foreach (var trip in completedTrips)
                {
                    if (trip.StartTime.HasValue)
                    {
                        var startTime = trip.StartTime.Value;

                        // Hourly analysis
                        var hour = startTime.Hour;
                        if (hourlyDemand.ContainsKey(hour))
                            hourlyDemand[hour]++;
                        else
                            hourlyDemand[hour] = 1;

                        // Daily analysis
                        var dayOfWeek = startTime.DayOfWeek;
                        if (dailyDemand.ContainsKey(dayOfWeek))
                            dailyDemand[dayOfWeek]++;
                        else
                            dailyDemand[dayOfWeek] = 1;

                        // Monthly analysis
                        var month = startTime.Month;
                        if (monthlyDemand.ContainsKey(month))
                            monthlyDemand[month]++;
                        else
                            monthlyDemand[month] = 1;
                    }
                }

                // Peak hours analysis
                Console.WriteLine("Peak Hours (24-hour format):");
                var peakHours = hourlyDemand.OrderByDescending(x => x.Value).Take(5);
                foreach (var hour in peakHours)
                {
                    var timeStr = $"{hour.Key:00}:00 - {hour.Key:00}:59";
                    Console.WriteLine($"  {timeStr}: {hour.Value} trips");
                }

                Console.WriteLine();

                // Peak days analysis
                Console.WriteLine("Peak Days of the Week:");
                var peakDays = dailyDemand.OrderByDescending(x => x.Value);
                foreach (var day in peakDays)
                {
                    Console.WriteLine($"  {day.Key}: {day.Value} trips");
                }

                Console.WriteLine();

                // Peak months analysis
                Console.WriteLine("Peak Months:");
                var peakMonths = monthlyDemand.OrderByDescending(x => x.Value);
                foreach (var month in peakMonths)
                {
                    var monthName = new DateTime(2024, month.Key, 1).ToString("MMMM");
                    Console.WriteLine($"  {monthName}: {month.Value} trips");
                }

                Console.WriteLine();

                // 3. Combined analysis - City and Time
                Console.WriteLine("3. CITY-TIME COMBINED ANALYSIS");
                Console.WriteLine("===============================");

                // Find peak demand times for top cities
                var top3Cities = topCities.Take(3);
                foreach (var city in top3Cities)
                {
                    Console.WriteLine($"Peak Hours for {city.Key}:");
                    
                    var cityTrips = completedTrips.Where(t => 
                        (locationDict.ContainsKey(t.FromLocation.Id) && locationDict[t.FromLocation.Id].City == city.Key) ||
                        (locationDict.ContainsKey(t.ToLocation.Id) && locationDict[t.ToLocation.Id].City == city.Key))
                        .Where(t => t.StartTime.HasValue)
                        .ToList();

                    var cityHourlyDemand = new Dictionary<int, int>();
                    foreach (var trip in cityTrips)
                    {
                        var hour = trip.StartTime.Value.Hour;
                        if (cityHourlyDemand.ContainsKey(hour))
                            cityHourlyDemand[hour]++;
                        else
                            cityHourlyDemand[hour] = 1;
                    }

                    var cityPeakHours = cityHourlyDemand.OrderByDescending(x => x.Value).Take(3);
                    foreach (var hour in cityPeakHours)
                    {
                        var timeStr = $"{hour.Key:00}:00 - {hour.Key:00}:59";
                        Console.WriteLine($"  {timeStr}: {hour.Value} trips");
                    }
                    Console.WriteLine();
                }

                // 4. Demand patterns summary
                Console.WriteLine("4. DEMAND PATTERNS SUMMARY");
                Console.WriteLine("==========================");

                var totalTrips = completedTrips.Count;
                var peakHour = hourlyDemand.OrderByDescending(x => x.Value).First();
                var peakDay = dailyDemand.OrderByDescending(x => x.Value).First();
                var peakMonth = monthlyDemand.OrderByDescending(x => x.Value).First();
                var topCity = topCities.First();

                Console.WriteLine($"Total Trips Analyzed: {totalTrips}");
                Console.WriteLine($"Peak Hour: {peakHour.Key:00}:00 ({peakHour.Value} trips)");
                Console.WriteLine($"Peak Day: {peakDay.Key} ({peakDay.Value} trips)");
                Console.WriteLine($"Peak Month: {new DateTime(2024, peakMonth.Key, 1).ToString("MMMM")} ({peakMonth.Value} trips)");
                Console.WriteLine($"Highest Demand City: {topCity.Key} ({topCity.Value} trips)");

                // Calculate demand intensity
                var averageHourlyDemand = hourlyDemand.Values.Average();
                var peakHourIntensity = (double)peakHour.Value / averageHourlyDemand;
                Console.WriteLine($"Peak Hour Demand Intensity: {peakHourIntensity:F1}x average");

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
