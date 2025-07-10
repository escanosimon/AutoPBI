using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace AutoPBI.Services
{
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    public class ObservableHashMap<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
        where TKey : notnull
    {
        private readonly Dictionary<TKey, TValue> _dictionary;
        private SimpleMonitor? _monitor; // Lazily allocated for reentrancy
        [NonSerialized]
        private int _blockReentrancyCount;

        // Constructors
        public ObservableHashMap()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        public ObservableHashMap(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            _dictionary = new Dictionary<TKey, TValue>(collection ?? throw new ArgumentNullException(nameof(collection)));
        }

        public ObservableHashMap(IDictionary<TKey, TValue> dictionary)
        {
            _dictionary = new Dictionary<TKey, TValue>(dictionary ?? throw new ArgumentNullException(nameof(dictionary)));
        }

        // INotifyCollectionChanged
        [field: NonSerialized]
        public virtual event NotifyCollectionChangedEventHandler? CollectionChanged;

        // INotifyPropertyChanged
        [field: NonSerialized]
        protected virtual event PropertyChangedEventHandler? PropertyChanged;

        event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
        {
            add => PropertyChanged += value;
            remove => PropertyChanged -= value;
        }

        // IDictionary<TKey, TValue> implementation
        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                CheckReentrancy();
                if (_dictionary.ContainsKey(key))
                {
                    TValue oldValue = _dictionary[key];
                    _dictionary[key] = value;
                    OnIndexerPropertyChanged();
                    OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, oldValue), new KeyValuePair<TKey, TValue>(key, value), -1);
                }
                else
                {
                    _dictionary[key] = value;
                    OnCountPropertyChanged();
                    OnIndexerPropertyChanged();
                    OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value), -1);
                }
            }
        }

        public int Count => _dictionary.Count;

        public bool IsReadOnly => ((IDictionary<TKey, TValue>)_dictionary).IsReadOnly;

        public ICollection<TKey> Keys => _dictionary.Keys;

        public ICollection<TValue> Values => _dictionary.Values;

        public void Add(TKey key, TValue value)
        {
            CheckReentrancy();
            _dictionary.Add(key, value);
            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value), -1);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            CheckReentrancy();
            ((IDictionary<TKey, TValue>)_dictionary).Add(item);
            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, -1);
        }

        public void Clear()
        {
            CheckReentrancy();
            if (_dictionary.Count > 0)
            {
                _dictionary.Clear();
                OnCountPropertyChanged();
                OnIndexerPropertyChanged();
                OnCollectionReset();
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)_dictionary).Contains(item);

        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((IDictionary<TKey, TValue>)_dictionary).CopyTo(array, arrayIndex);

        public bool Remove(TKey key)
        {
            CheckReentrancy();
            if (_dictionary.TryGetValue(key, out TValue? value))
            {
                _dictionary.Remove(key);
                OnCountPropertyChanged();
                OnIndexerPropertyChanged();
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value), -1);
                return true;
            }
            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            CheckReentrancy();
            if (((IDictionary<TKey, TValue>)_dictionary).Remove(item))
            {
                OnCountPropertyChanged();
                OnIndexerPropertyChanged();
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, -1);
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value!);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_dictionary).GetEnumerator();

        // Event raising methods
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler? handler = CollectionChanged;
            if (handler != null)
            {
                _blockReentrancyCount++;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    _blockReentrancyCount--;
                }
            }
        }

        private void OnCountPropertyChanged() => OnPropertyChanged(EventArgsCache.CountPropertyChanged);

        private void OnIndexerPropertyChanged() => OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);

        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> item, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> oldItem, KeyValuePair<TKey, TValue> newItem, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        private void OnCollectionReset() => OnCollectionChanged(EventArgsCache.ResetCollectionChanged);

        // Reentrancy protection
        protected IDisposable BlockReentrancy()
        {
            _blockReentrancyCount++;
            return EnsureMonitorInitialized();
        }

        protected void CheckReentrancy()
        {
            if (_blockReentrancyCount > 0)
            {
                NotifyCollectionChangedEventHandler? handler = CollectionChanged;
                if (handler != null && !handler.HasSingleTarget())
                    throw new InvalidOperationException("ObservableHashMap reentrancy not allowed.");
            }
        }

        private SimpleMonitor EnsureMonitorInitialized() => _monitor ??= new SimpleMonitor(this);

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            EnsureMonitorInitialized();
            _monitor!._busyCount = _blockReentrancyCount;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_monitor != null)
            {
                _blockReentrancyCount = _monitor._busyCount;
                _monitor._collection = this;
            }
        }

        // Reentrancy monitor
        [Serializable]
        private sealed class SimpleMonitor : IDisposable
        {
            internal int _busyCount;
            [NonSerialized]
            internal ObservableHashMap<TKey, TValue> _collection;

            public SimpleMonitor(ObservableHashMap<TKey, TValue> collection)
            {
                _collection = collection ?? throw new ArgumentNullException(nameof(collection));
            }

            public void Dispose() => _collection._blockReentrancyCount--;
        }
    }

    // Reuse EventArgsCache from ObservableCollection<T>
    internal static class EventArgsCache
    {
        internal static readonly PropertyChangedEventArgs CountPropertyChanged = new PropertyChangedEventArgs("Count");
        internal static readonly PropertyChangedEventArgs IndexerPropertyChanged = new PropertyChangedEventArgs("Item[]");
        internal static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
    }

    // Extension method to check if event handler has a single target (for reentrancy)
    internal static class NotifyCollectionChangedEventHandlerExtensions
    {
        public static bool HasSingleTarget(this NotifyCollectionChangedEventHandler? handler)
        {
            if (handler == null) return true;
            Delegate[]? invocationList = handler.GetInvocationList();
            return invocationList.Length <= 1;
        }
    }
}