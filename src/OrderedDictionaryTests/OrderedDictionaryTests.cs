using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OrderedDictionary;

namespace OrderedDictionaryTests
{
    public class OrderedDictionaryTests
    {
        [Test]
        public void UsualScenario_EnumeratePreserveOrdering()
        {
            // arrange
            var addList = new[] {1, 2, 3, 4, 5};
            var orderedDictionary = new OrderedDictionary<int, int>();
            var dictionary = new Dictionary<int, int>();

            // act reproducing loose ordering from here https://stackoverflow.com/questions/1453190/does-the-enumerator-of-a-dictionarytkey-tvalue-return-key-value-pairs-in-the
            foreach (var item in addList[..^1])
            {
                orderedDictionary.Add(item, item);
                dictionary.Add(item, item);
            }

            dictionary.Remove(addList.First());
            dictionary[addList.Last()] = addList.Last();

            orderedDictionary.Remove(addList.First());
            orderedDictionary[addList.Last()] = addList.Last();

            var checkList = addList[1..].ToList();

            // assert
            var listIndex = 0;
            var anyDictionaryNotEquals = false;
            foreach (var nonOrderedElement in dictionary)
            {
                anyDictionaryNotEquals = !nonOrderedElement.Key.Equals(checkList[listIndex]);

                if (anyDictionaryNotEquals)
                {
                    break;
                }

                listIndex++;
            }

            Assert.IsTrue(anyDictionaryNotEquals);

            listIndex = 0;
            foreach (var orderedElement in orderedDictionary)
            {
                Assert.AreEqual(orderedElement.Key, checkList[listIndex]);
                listIndex++;
            }
        }
    }
}