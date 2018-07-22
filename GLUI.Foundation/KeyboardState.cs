using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GLUI.Foundation
{
    public delegate void KeyboardHandler(object sender, KeyboardState keyboardState);
    public class KeyboardState : EventArgs
    {
        private class OwnDictionary<K, V> : IDictionary<K, V>
        {
            public delegate void DictionaryChanged(object sender, (K key, V value) e);
            public event DictionaryChanged OnDictionaryChanged;

            private Dictionary<K, V> mInner = new Dictionary<K, V>();

            public V this[K key]
            {
                get
                {
                    return mInner[key];
                }
                set
                {
                    mInner[key] = value; OnDictionaryChanged?.Invoke(this, (key, value));
                }
            }
            public ICollection<K> Keys => mInner.Keys;
            public ICollection<V> Values => mInner.Values;
            public int Count => mInner.Count;
            public void Add(K key, V value) => mInner.Add(key, value);
            public void Add(KeyValuePair<K, V> item) => mInner.Add(item.Key, item.Value);
            public void Clear() => mInner.Clear();
            public bool Contains(KeyValuePair<K, V> item) => mInner.Contains(item);
            public bool ContainsKey(K key) => mInner.ContainsKey(key);
            public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => mInner.GetEnumerator();
            public bool Remove(K key) => mInner.Remove(key);
            public bool Remove(KeyValuePair<K, V> item) => mInner.Remove(item.Key);
            public bool TryGetValue(K key, out V value) => mInner.TryGetValue(key, out value);
            IEnumerator IEnumerable.GetEnumerator() => mInner.GetEnumerator();
            public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) => throw new NotImplementedException();
            public bool IsReadOnly => throw new NotImplementedException();
        }

        public bool Alt { get; set; }
        public bool Control { get; set; }
        public bool Shift { get; set; }
        public char KeyChar { get; set; }
        public bool IsPressed { get; set; }
        public IDictionary<Key, bool> KeyDown { get; } = new OwnDictionary<Key, bool>();
        public List<Key> Last10Keys { get; } = new List<Key>();
        public Key LastKey
        {
            get
            {
                return Last10Keys.LastOrDefault();
            }
            private set
            {
                Last10Keys.Add(value);
                if (Last10Keys.Count > 10) Last10Keys.RemoveAt(0);
            }
        }

        public KeyboardState()
        {
            (KeyDown as OwnDictionary<Key, bool>).OnDictionaryChanged += (o, e) =>
            {
                if (e.value) LastKey = e.key;
            };
            var wProperties = typeof(Key).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty);
            foreach (var wProperty in wProperties)
            {
                KeyDown[wProperty.GetValue(null) as Key] = false;
            }
        }
    }
}
