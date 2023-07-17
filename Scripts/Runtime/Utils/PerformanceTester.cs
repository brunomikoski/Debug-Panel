using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrunoMikoski.DebugTools
{
    public static class PerformanceTester
    {
        [Serializable]
        public struct PerformanceResult
        {
            public double MedianTime;
            public double MeanTime;
            public double MinTime;
            public double MaxTime;
            public double Range;
            public double TotalTime;
            public int NumberOfRuns;
            public long ExecutionDateTime;

            public override string ToString()
            {
                StringBuilder finalResult = new StringBuilder();
                finalResult.AppendLine($"------------------- {NumberOfRuns} RUNS {DateTime.FromBinary(ExecutionDateTime)}-------------------");
                finalResult.AppendLine($"Median Time: {MedianTime} ms");
                finalResult.AppendLine($"Mean Time: {MeanTime} ms");
                finalResult.AppendLine($"Min Time: {MinTime} ms");
                finalResult.AppendLine($"Max Time: {MaxTime} ms");
                finalResult.AppendLine($"Range: {Range} ms");
                finalResult.AppendLine($"Total Time: {TotalTime} ms");
                finalResult.AppendLine($"---------------------------------------------------------");
                return finalResult.ToString();
            }
        }
        
        [Serializable]
        public struct PerformanceComparison
        {
            public double MedianTimeDifference;
            public double MeanTimeDifference;
            public double MinTimeDifference;
            public double MaxTimeDifference;
            public double RangeDifference;
            public double TotalTimeDifference;
        }
        

        public static PerformanceResult RunTest(Action action, int runs)
        {
            // Warm up
            action();
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            List<double> durations = new List<double>(runs);

            // Call Garbage Collector to minimize interference
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            
            for (int i = 0; i < runs; i++)
            {
                stopwatch.Restart();
                
                action();

                stopwatch.Stop();
                durations.Add(stopwatch.Elapsed.TotalMilliseconds);
            }

            double totalDuration = durations.Sum();
            double meanDuration = totalDuration / runs;
            double minDuration = durations.Min();
            double maxDuration = durations.Max();
            double medianDuration;
            double range = maxDuration - minDuration;

            List<double> sortedDurations = durations.OrderBy(x => x).ToList();
            if (sortedDurations.Count % 2 == 0)
            {
                // Even number of observations
                int midIndex = sortedDurations.Count / 2;
                medianDuration = (sortedDurations[midIndex] + sortedDurations[midIndex - 1]) / 2.0;
            }
            else
            {
                // Odd number of observations
                medianDuration = sortedDurations[sortedDurations.Count / 2];
            }

            PerformanceResult result = new PerformanceResult
            {
                MedianTime = medianDuration,
                MeanTime = meanDuration,
                MinTime = minDuration,
                MaxTime = maxDuration,
                Range = range,
                TotalTime = totalDuration,
                NumberOfRuns = runs,
                ExecutionDateTime = DateTime.UtcNow.ToBinary()
            };

            return result;
        }
        
        public static PerformanceComparison ComparePerformance(PerformanceResult first, PerformanceResult second)
        {
            PerformanceComparison comparison = new PerformanceComparison
            {
                MedianTimeDifference = CalculatePercentageDifference(first.MedianTime, second.MedianTime),
                MeanTimeDifference = CalculatePercentageDifference(first.MeanTime, second.MeanTime),
                MinTimeDifference = CalculatePercentageDifference(first.MinTime, second.MinTime),
                MaxTimeDifference = CalculatePercentageDifference(first.MaxTime, second.MaxTime),
                RangeDifference = CalculatePercentageDifference(first.Range, second.Range),
                TotalTimeDifference = CalculatePercentageDifference(first.TotalTime, second.TotalTime)
            };

            return comparison;
        }

        private static double CalculatePercentageDifference(double first, double second)
        {
            if (first == 0 && second == 0) return 0;

            if (first == 0) return 100;

            return ((second - first) / first) * 100;
        }
    }
}
