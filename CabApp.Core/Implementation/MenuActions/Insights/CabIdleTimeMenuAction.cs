using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Insights
{
    public class CabIdleTimeMenuAction: IMenuAction
    {
        private readonly IDataService _dataService;
        private readonly IAppLogger _appLogger;

        public CabIdleTimeMenuAction(IDataService dataService, IAppLogger appLogger)
        {
            _dataService = dataService;
            _appLogger = appLogger;
        }

        public string Title => "Cab Idle Time Analysis";
        public string Description => "Analyze how much time cabs were idle in a given duration";
        public char KeyChar => '1';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== Cab Idle Time Analysis ===");
                Console.WriteLine();

                // Get date range from user
                Console.WriteLine("Enter the analysis period:");
                Console.Write("Start Date (yyyy-mm-dd): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
                {
                    Console.WriteLine("Invalid start date format.");
                    return false;
                }

                Console.Write("End Date (yyyy-mm-dd): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
                {
                    Console.WriteLine("Invalid end date format.");
                    return false;
                }

                if (startDate >= endDate)
                {
                    Console.WriteLine("Start date must be before end date.");
                    return false;
                }

                Console.WriteLine();
                Console.WriteLine("Analyzing cab idle time...");
                Console.WriteLine();

                // Get all cabs and trips
                var cabs = await _dataService.GetAllCabsAsync();
                var trips = await _dataService.GetAllTripsAsync();

                // Filter completed trips within the date range
                var completedTrips = trips.Where(t => 
                    t.TripStatus == TripStatus.COMPLETED && 
                    t.StartTime.HasValue && 
                    t.EndTime.HasValue &&
                    t.StartTime.Value.Date >= startDate.Date && 
                    t.EndTime.Value.Date <= endDate.Date.AddDays(1)).ToList();

                Console.WriteLine($"Analysis Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
                Console.WriteLine($"Total Completed Trips: {completedTrips.Count}");
                Console.WriteLine();

                // Calculate idle time for each cab
                var cabIdleAnalysis = new List<(int CabId, TimeSpan TotalIdleTime, int TripCount)>();

                foreach (var cab in cabs)
                {
                    var cabTrips = completedTrips.Where(t => t.AssignedCabId == cab.Id)
                                                .OrderBy(t => t.StartTime)
                                                .ToList();

                    if (cabTrips.Count == 0)
                    {
                        // Cab had no trips in this period
                        var totalPeriod = endDate - startDate;
                        cabIdleAnalysis.Add((cab.Id, totalPeriod, 0));
                        continue;
                    }

                    TimeSpan cabTotalIdleTime = TimeSpan.Zero;

                    // Idle time before first trip
                    var firstTrip = cabTrips.First();
                    if (firstTrip.StartTime.HasValue)
                    {
                        var idleBeforeFirst = firstTrip.StartTime.Value - startDate;
                        if (idleBeforeFirst > TimeSpan.Zero)
                        {
                            cabTotalIdleTime += idleBeforeFirst;
                        }
                    }

                    // Idle time between trips
                    for (int i = 0; i < cabTrips.Count - 1; i++)
                    {
                        var currentTrip = cabTrips[i];
                        var nextTrip = cabTrips[i + 1];

                        if (currentTrip.EndTime.HasValue && nextTrip.StartTime.HasValue)
                        {
                            var idleBetween = nextTrip.StartTime.Value - currentTrip.EndTime.Value;
                            if (idleBetween > TimeSpan.Zero)
                            {
                                cabTotalIdleTime += idleBetween;
                            }
                        }
                    }

                    // Idle time after last trip
                    var lastTrip = cabTrips.Last();
                    if (lastTrip.EndTime.HasValue)
                    {
                        var idleAfterLast = endDate - lastTrip.EndTime.Value;
                        if (idleAfterLast > TimeSpan.Zero)
                        {
                            cabTotalIdleTime += idleAfterLast;
                        }
                    }

                    cabIdleAnalysis.Add((cab.Id, cabTotalIdleTime, cabTrips.Count));
                }

                // Sort by total idle time (descending)
                cabIdleAnalysis = cabIdleAnalysis.OrderByDescending(x => x.TotalIdleTime).ToList();

                Console.WriteLine("Cab Idle Time Analysis Results:");
                Console.WriteLine("=================================");
                Console.WriteLine($"{"Cab ID",-8} {"Total Idle Time",-20} {"Trips Count",-12} {"Idle %",-10}");
                Console.WriteLine(new string('-', 60));

                var totalPeriodDuration = endDate - startDate;
                var totalDays = totalPeriodDuration.TotalDays;

                foreach (var analysis in cabIdleAnalysis)
                {
                    var idlePercentage = totalDays > 0 ? (analysis.TotalIdleTime.TotalHours / (totalDays * 24)) * 100 : 0;
                    Console.WriteLine($"{analysis.CabId,-8} {FormatTimeSpan(analysis.TotalIdleTime),-20} {analysis.TripCount,-12} {idlePercentage:F1}%");
                }

                Console.WriteLine();
                Console.WriteLine("Summary:");
                var totalIdleTime = TimeSpan.FromTicks(cabIdleAnalysis.Sum(x => x.TotalIdleTime.Ticks));
                var totalTrips = cabIdleAnalysis.Sum(x => x.TripCount);
                var averageIdleTime = cabIdleAnalysis.Count > 0 ? TimeSpan.FromTicks((long)cabIdleAnalysis.Average(x => x.TotalIdleTime.Ticks)) : TimeSpan.Zero;

                Console.WriteLine($"Total Idle Time Across All Cabs: {FormatTimeSpan(totalIdleTime)}");
                Console.WriteLine($"Average Idle Time Per Cab: {FormatTimeSpan(averageIdleTime)}");
                Console.WriteLine($"Total Trips Completed: {totalTrips}");
                var avgTripsPerCab = cabIdleAnalysis.Count > 0 ? (double)totalTrips / cabIdleAnalysis.Count : 0;
                Console.WriteLine($"Average Trips Per Cab: {avgTripsPerCab:F1}");

                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in CabIdleTimeMenuAction", ex);
                Console.WriteLine("An error occurred while analyzing cab idle time.");
                return false;
            }
        }

        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
            {
                return $"{timeSpan.Days}d {timeSpan.Hours}h {timeSpan.Minutes}m";
            }
            else if (timeSpan.TotalHours >= 1)
            {
                return $"{timeSpan.Hours}h {timeSpan.Minutes}m";
            }
            else
            {
                return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
            }
        }
    }
}
