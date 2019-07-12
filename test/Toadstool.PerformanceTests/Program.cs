using System;
using BenchmarkDotNet.Running;

namespace Toadstool.PerformanceTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ToadstoolBenchmark>();
        }
    }
}
