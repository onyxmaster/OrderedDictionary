using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace OrderedDictionaryTests
{
    public class OrderedDictionaryAdvancedTests
    {
        [Test]
        public void IsRemoveConstantTime()
        {
            var rng = new Random();
            DictionaryTesting.TestComplexityGrowth(
                DictionaryTesting.CreateOrderedDictionary,
                dictionary => dictionary.Remove(rng.Next()),
                (dictionary, count) =>
                {
                    for (var i = 0; i < count; i++)
                    {
                        dictionary.Remove(rng.Next(count));
                    }
                });
        }

        [Test]
        public void IsKvpRemoveConstantTime()
        {
            var rng = new Random();
            DictionaryTesting.TestComplexityGrowth(
                DictionaryTesting.CreateOrderedDictionary,
                dictionary =>
                {
                    var j = rng.Next(dictionary.Count);
                    var kvp = KeyValuePair.Create(j, j);
                    dictionary.Remove(kvp);
                },
                (dictionary, count) =>
                {
                    for (var i = 0; i < count; i++)
                    {
                        var j = rng.Next(count);
                        var kvp = KeyValuePair.Create(j, j);
                        dictionary.Remove(kvp);
                    }
                });
        }
    }
}