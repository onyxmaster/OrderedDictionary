using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using NUnit.Framework;

using OrderedDictionary;

namespace OrderedDictionaryTests
{
    internal static class DictionaryTesting
    {
        public static int TestPerformance(Func<IDictionary<int, int>> create, Action<IDictionary<int, int>> act)
        {
            CheckOptimized();
            var dictionary = create();
            var count = 0;
            var monotonicTimer = new Stopwatch();
            act(dictionary);
            ForceFullGC();
            monotonicTimer.Start();
            do
            {
                act(dictionary);
                ++count;
            } while (monotonicTimer.ElapsedMilliseconds < 2000);

            return count;
        }

        public static void TestComplexityGrowth(Func<int, IDictionary<int, int>> create, Action<IDictionary<int, int>> arrange, Action<IDictionary<int, int>, int> act)
        {
            CheckOptimized();
            var previousSpentTime = TimeSpan.MaxValue;
            var monotonicTimer = new Stopwatch();
            for (var capacity = 10000; capacity < 10000000; capacity *= 2)
            {
                var dictionary = create(capacity);
                arrange(dictionary);
                var count = dictionary.Count;
                ForceFullGC();
                monotonicTimer.Restart();
                act(dictionary, count);
                var spentTime = monotonicTimer.Elapsed;
                monotonicTimer.Stop();
                var ratio = spentTime.TotalSeconds / previousSpentTime.TotalSeconds;
                Assert.Less(ratio, 4, "The time spent ratio {0} ({3}/{4}) is too large for capacity {2}->{1}, count {5}.", ratio, capacity, capacity / 2, spentTime, previousSpentTime, count);
                previousSpentTime = spentTime;
            }
        }

        public static IDictionary<int, int> CreateReferenceDictionary(int capacity)
        {
            var dictionary = new Dictionary<int, int>(capacity);
            PopulateDictionary(dictionary, capacity);
            return dictionary;
        }

        public static IDictionary<int, int> CreateOrderedDictionary(int capacity)
        {
            var dictionary = new OrderedDictionary<int, int>(capacity);
            PopulateDictionary(dictionary, capacity);
            return dictionary;
        }

        private static void PopulateDictionary(IDictionary<int, int> dictionary, int capacity)
        {
            for (var j = 0; j < capacity; j++)
            {
                dictionary.TryAdd(j, j);
            }
        }

        private static void ForceFullGC()
        {
            GC.Collect(2, GCCollectionMode.Forced, true, true);
        }

        private static void CheckOptimized()
        {
            var isDebuggable = typeof(OrderedDictionary<,>).Assembly
                .GetCustomAttributes(typeof(DebuggableAttribute), false)
                .Cast<DebuggableAttribute>()
                .Any(attr => (attr.DebuggingFlags & DebuggableAttribute.DebuggingModes.DisableOptimizations) == DebuggableAttribute.DebuggingModes.DisableOptimizations);
            Assert.False(isDebuggable, "Non-optimized builds cannot be reliably performance-tested.");
        }

    }
}
