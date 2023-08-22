using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Serialization;

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
            [FormerlySerializedAs("memoryAllocation")] public long MemoryAllocation;

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
                finalResult.AppendLine($"Total Heap Memory Allocated: {MemoryAllocation} bytes");
                finalResult.AppendLine($"---------------------------------------------------------");
                return finalResult.ToString();
            }

            public string StringResultComparedTo(PerformanceResult previous)
            {
                PerformanceComparison comparison = ComparePerformance(previous, this);
                
                StringBuilder finalResult = new StringBuilder();
                finalResult.AppendLine($"------------------- {NumberOfRuns} RUNS {DateTime.FromBinary(ExecutionDateTime)}-------------------");
                finalResult.AppendLine($"Median Time: {MedianTime} ms {GetChangeString(comparison.MedianTimeDifference)}");
                finalResult.AppendLine($"Mean Time: {MeanTime} ms {GetChangeString(comparison.MeanTimeDifference)}");
                finalResult.AppendLine($"Min Time: {MinTime} ms {GetChangeString(comparison.MinTimeDifference)}");
                finalResult.AppendLine($"Max Time: {MaxTime} ms {GetChangeString(comparison.MaxTimeDifference)}");
                finalResult.AppendLine($"Range: {Range} ms {GetChangeString(comparison.RangeDifference)}");
                finalResult.AppendLine($"Total Time: {TotalTime} ms {GetChangeString(comparison.TotalTimeDifference)}");
                finalResult.AppendLine($"Heap memory Difference: {MemoryAllocation} bytes {GetChangeString(comparison.MemoryAllocationDifference)}");
                finalResult.AppendLine($"---------------------------------------------------------");
                return finalResult.ToString();
            }

            private string GetChangeString(double value)
            {
                string color = value < 0 ? "green" : "red";
                return $"<color={color}>[{value:00.00}%]</color>";
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
            public double MemoryAllocationDifference;
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
            long initialMemory = GC.GetTotalMemory(false);

            
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

            long finalMemory = GC.GetTotalMemory(false);
            PerformanceResult result = new PerformanceResult
            {
                MedianTime = medianDuration,
                MeanTime = meanDuration,
                MinTime = minDuration,
                MaxTime = maxDuration,
                Range = range,
                TotalTime = totalDuration,
                NumberOfRuns = runs,
                ExecutionDateTime = DateTime.Now.ToBinary(),
                MemoryAllocation = finalMemory-initialMemory
            };

            return result;
        }
        
        public static PerformanceComparison ComparePerformance(PerformanceResult previousResult, PerformanceResult currentResult)
        {
            PerformanceComparison comparison = new PerformanceComparison
            {
                MedianTimeDifference = CalculatePercentageDifference(previousResult.MedianTime, currentResult.MedianTime),
                MeanTimeDifference = CalculatePercentageDifference(previousResult.MeanTime, currentResult.MeanTime),
                MinTimeDifference = CalculatePercentageDifference(previousResult.MinTime, currentResult.MinTime),
                MaxTimeDifference = CalculatePercentageDifference(previousResult.MaxTime, currentResult.MaxTime),
                RangeDifference = CalculatePercentageDifference(previousResult.Range, currentResult.Range),
                TotalTimeDifference = CalculatePercentageDifference(previousResult.TotalTime, currentResult.TotalTime),
                MemoryAllocationDifference = CalculatePercentageDifference(previousResult.MemoryAllocation, currentResult.MemoryAllocation)
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
