using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace OrderedDictionaryTests
{
    public class OrderedDictionaryExtraTests
    {
        [Test]
        public void EnumerationWorksAfterConflictingKvpInsert()
        {
            var orderedDictionary = DictionaryTesting.CreateOrderedDictionary(0);
            var kvp = KeyValuePair.Create(1, 1);
            orderedDictionary.Add(kvp);
            Assert.Throws<ArgumentException>(() => orderedDictionary.Add(kvp));
            var orderedCount = orderedDictionary.Count(_ => true);
            Assert.AreEqual(1, orderedCount);
        }

        [Test]
        public void IsNonExistentKeyRemoveConstantTime()
        {
            DictionaryTesting.TestComplexityGrowth(
                DictionaryTesting.CreateOrderedDictionary,
                dictionary => dictionary.Remove(0),
                (dictionary, count) =>
                {
                    for (var i = 0; i < count; i++)
                    {
                        dictionary.Remove(0);
                    }
                });
        }

        [Test]
        public void IsNonExistentKvpRemoveConstantTime()
        {
            var kvp = KeyValuePair.Create(1, 1);
            DictionaryTesting.TestComplexityGrowth(
                DictionaryTesting.CreateOrderedDictionary,
                dictionary => dictionary.Remove(kvp),
                (dictionary, count) =>
                {
                    for (var i = 0; i < count; i++)
                    {
                        dictionary.Remove(kvp);
                    }
                });
        }

        [Test]
        public void IsContainsKvpOptimallyImplemented()
        {
            var kvp = KeyValuePair.Create(1, 1);
            const int Capacity = 1000000;
            Action<IDictionary<int, int>> act = d => d.Contains(kvp);
            var orderedDictionaryCount = DictionaryTesting.TestPerformance(
                () => DictionaryTesting.CreateOrderedDictionary(Capacity), 
                act);
            var referenceDictionaryCount = DictionaryTesting.TestPerformance(
                () => DictionaryTesting.CreateReferenceDictionary(Capacity),
                act);
            Assert.AreEqual(
                orderedDictionaryCount,
                referenceDictionaryCount,
                referenceDictionaryCount * 0.08,
                "The time-based test performance for Contains(KeyValuePair<,>) in ordered dictionary deviates from reference one too much.");
        }
    }
}