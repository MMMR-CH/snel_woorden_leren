using System;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Utility
{
    [Serializable]
    public class SerializableKeyValuePair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;

        public SerializableKeyValuePair()
        {
        }

        public SerializableKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<SerializableKeyValuePair<TKey, TValue>> pairs = 
            new List<SerializableKeyValuePair<TKey, TValue>>();

        private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public Dictionary<TKey, TValue> Dictionary => dictionary;

        public void OnBeforeSerialize()
        {
            // Nothing needed here as we maintain the pairs list directly
        }

        public void OnAfterDeserialize()
        {
            dictionary.Clear();
            var seenKeys = new HashSet<TKey>();
            foreach (var pair in pairs)
            {
                if (pair.Key != null && !seenKeys.Contains(pair.Key))
                {
                    dictionary.Add(pair.Key, pair.Value);
                    seenKeys.Add(pair.Key);
                }
                // Optionally, you could log a warning if a duplicate is found
                // else
                //     Debug.LogWarning($"Duplicate key found in SerializableDictionary: {pair.Key}");
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                throw new ArgumentException($"An element with the same key already exists: {key}");

            var pair = new SerializableKeyValuePair<TKey, TValue>(key, value);
            pairs.Add(pair);
            dictionary[key] = value;
        }

        public void Clear()
        {
            pairs.Clear();
            dictionary.Clear();
        }

        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

        public bool Remove(TKey key)
        {
            pairs.RemoveAll(p => EqualityComparer<TKey>.Default.Equals(p.Key, key));
            return dictionary.Remove(key);
        }

        public TValue this[TKey key]
        {
            get => dictionary[key];
            set
            {
                dictionary[key] = value;
                var existingPair = pairs.Find(p => EqualityComparer<TKey>.Default.Equals(p.Key, key));
                if (existingPair != null)
                {
                    existingPair.Value = value;
                }
                else
                {
                    pairs.Add(new SerializableKeyValuePair<TKey, TValue>(key, value));
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dictionary.GetEnumerator();
    }
}