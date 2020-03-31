using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OrderedDictionary
{
    /// <summary>
    /// IDictionary implementation with ordering preserve.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly LinkedList<TKey> _orderedKeys;
        private readonly Dictionary<TKey, TValue> _innerDictionary;

        private readonly KeyCollection _keyCollection;
        private readonly ValueCollection _valueCollection;

        public OrderedDictionary() : this(0, null)
        {
        }

        public OrderedDictionary(int capacity) : this(capacity, null)
        {
        }

        public OrderedDictionary(IEqualityComparer<TKey>? comparer) : this(0, comparer)
        {
        }

        public OrderedDictionary(int capacity, IEqualityComparer<TKey>? comparer)
        {
            _innerDictionary = new Dictionary<TKey, TValue>(capacity, comparer);
            _orderedKeys = new LinkedList<TKey>();

            _keyCollection = new KeyCollection(_orderedKeys, _innerDictionary);
            _valueCollection = new ValueCollection(_orderedKeys, _innerDictionary);
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _orderedKeys.Select(k => new KeyValuePair<TKey, TValue>(k, _innerDictionary[k])).GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _innerDictionary.Add(item.Key, item.Value);
            _orderedKeys.AddLast(item.Key);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _innerDictionary.Clear();
            _orderedKeys.Clear();
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, TValue> item) => _innerDictionary.Contains(item);

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if ((uint) index > array.Length)
            {
                throw new IndexOutOfRangeException("Index must be inside array boundary.");
            }

            if (array.Length - index < _orderedKeys.Count)
            {
                throw new ArgumentException("Array must have available space for copying.");
            }

            foreach (var key in _orderedKeys)
            {
                // Not great not terrible.
                array[index++] = new KeyValuePair<TKey, TValue>(key, _innerDictionary[key]);
            }
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _innerDictionary.Remove(item.Key) && _orderedKeys.Remove(item.Key);
        }

        /// <inheritdoc />
        public int Count => _innerDictionary.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public void Add(TKey key, TValue value)
        {
            _innerDictionary.Add(key, value);
            _orderedKeys.AddLast(key);
        }

        /// <inheritdoc />
        public bool ContainsKey(TKey key)
        {
            return _innerDictionary.ContainsKey(key);
        }

        /// <inheritdoc />
        public bool Remove(TKey key)
        {
            return _innerDictionary.Remove(key) && _orderedKeys.Remove(key);
        }

        /// <inheritdoc />
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _innerDictionary.TryGetValue(key, out value);
        }

        /// <inheritdoc />
        public TValue this[TKey key]
        {
            get => _innerDictionary[key];
            set
            {
                // Maybe here we should place key to the end of the list in all cases and remove from inside.
                if (!_innerDictionary.ContainsKey(key))
                {
                    _orderedKeys.AddLast(key);
                }

                _innerDictionary[key] = value;
            }
        }

        /// <inheritdoc />
        public ICollection<TKey> Keys => _keyCollection;

        /// <inheritdoc />
        public ICollection<TValue> Values => _valueCollection;

        private sealed class KeyCollection : ICollection<TKey>
        {
            private readonly LinkedList<TKey> _innerList;
            private readonly Dictionary<TKey, TValue> _dictionary;

            public KeyCollection(LinkedList<TKey> innerList, Dictionary<TKey, TValue> dictionary)
            {
                _innerList = innerList;
                _dictionary = dictionary;
            }

            public void CopyTo(TKey[] array, int index)
            {
                _innerList.CopyTo(array, index);
            }

            public int Count => _innerList.Count;

            bool ICollection<TKey>.IsReadOnly => true;

            void ICollection<TKey>.Add(TKey item) => throw new NotSupportedException();

            void ICollection<TKey>.Clear() => throw new NotSupportedException();

            bool ICollection<TKey>.Contains(TKey item)
                => item != null && _dictionary.ContainsKey(item);

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new NotSupportedException();
            }

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => _innerList.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _innerList.GetEnumerator();
        }

        private sealed class ValueCollection : ICollection<TValue>
        {
            private readonly LinkedList<TKey> _innerList;
            private readonly Dictionary<TKey, TValue> _dictionary;

            public ValueCollection(LinkedList<TKey> innerList, Dictionary<TKey, TValue> dictionary)
            {
                _innerList = innerList;
                _dictionary = dictionary;
            }

            public void CopyTo(TValue[] array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if ((uint) index > array.Length)
                {
                    throw new IndexOutOfRangeException("Index must be inside array boundary.");
                }

                if (array.Length - index < _dictionary.Count)
                {
                    throw new ArgumentException("Array must have available space for copying.");
                }

                foreach (var key in _innerList)
                {
                    array[index++] = _dictionary[key];
                }
            }

            public int Count => _innerList.Count;

            bool ICollection<TValue>.IsReadOnly => true;

            void ICollection<TValue>.Add(TValue item) => throw new NotSupportedException();

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TValue>.Clear() => throw new NotSupportedException();

            bool ICollection<TValue>.Contains(TValue item)
                => _dictionary.ContainsValue(item);

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => _innerList.Select(k => _dictionary[k]).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _innerList.Select(k => _dictionary[k]).GetEnumerator();
        }
    }
}