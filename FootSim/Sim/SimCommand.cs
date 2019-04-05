﻿namespace FootSim.Sim
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using FootSim.Core;

    public static class SimCommand
    {
        public static async Task<int> RunAsync(SimOptions options)
        {
            var repository = new Repository(Constants.CsvFilePath, Constants.Url);

            Console.WriteLine($"Simulating {options.Simulations:N0} seasons ...");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var seasonSimulator = new SeasonSimulator(repository);
            var results = seasonSimulator.Simulate(options.Simulations, options.Until);

            stopwatch.Stop();

            Console.WriteLine();
            Console.WriteLine(GetHeaderLine());

            foreach (var keyValuePair in results.OrderByDescending(kvp => kvp.Value.AveragePoints))
            {
                var teamName = keyValuePair.Key;

                Console.WriteLine($"{teamName,-20} {GetDescription(keyValuePair.Value)}");
            }

            Console.WriteLine();
            Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");

            Console.ReadLine();

            return 0;
        }

        private static string GetDescription(SeasonSimulationResult seasonSimulationResult)
        {
            var stringBuilder = new StringBuilder();

            foreach (var position in Enumerable.Range(1, 20))
            {
                var proportion = seasonSimulationResult.PositionProportion(position);
                var percentage = proportion == 0 ? string.Empty : (proportion * 100).ToString(".0");

                stringBuilder.Append($"{percentage,5} ");
            }

            var avgPts = seasonSimulationResult.AveragePoints.ToString("N1");
            stringBuilder.Append($"{avgPts,8}");

            return stringBuilder.ToString();
        }

        private static string GetHeaderLine()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"{"Name",-20} ");

            foreach (var position in Enumerable.Range(1, 20))
            {
                var pos = $"#{position}";
                stringBuilder.Append($"{pos,5} ");
            }

            stringBuilder.Append(" Avg Pts");

            return stringBuilder.ToString();
        }
    }
}