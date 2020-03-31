using System.Collections.Generic;

using NUnit.Framework;

namespace OrderedDictionaryTests
{
    public class OrderedDictionaryAdvancedTests
    {
        [Test]
        public void IsRemoveConstantTime()
        {
            DictionaryTesting.TestComplexityGrowth(
                DictionaryTesting.CreateOrderedDictionary,
                dictionary => dictionary.Remove(-1),
                (dictionary, count) =>
                {
                    for (var i = 0; i < count; i++)
                    {
                        dictionary.Remove(i);
                    }
                });
        }

        [Test]
        public void IsKvpRemoveConstantTime()
        {
            DictionaryTesting.TestComplexityGrowth(
                DictionaryTesting.CreateOrderedDictionary,
                dictionary => dictionary.Remove(KeyValuePair.Create(-1, -1)),
                (dictionary, count) =>
                {
                    for (var i = 0; i < count; i++)
                    {
                        var kvp = KeyValuePair.Create(i, i);
                        dictionary.Remove(kvp);
                    }
                });
        }
    }
}