using System;
using BenchmarkDotNet.Running;

namespace Normal.PerformanceTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<NormalBenchmark>();
        }
    }
}
