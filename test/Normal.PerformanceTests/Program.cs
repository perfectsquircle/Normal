using System;
using BenchmarkDotNet.Running;

namespace Normal.PerformanceTests
{
    class Program
    {
        static void Main()
        {
            _ = BenchmarkRunner.Run<NormalBenchmark>();
        }
    }
}
