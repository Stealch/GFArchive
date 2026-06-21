using System;
using System.Collections.Generic;
using VitalEngine.Archive.Exceptions;

namespace VitalEngine.Archive.Core
{
    /// <summary>
    /// Трансляционная таблица (XlatTable) из игры
    /// Используется для трансляции REFID → данные
    /// </summary>
    /// <typeparam name="T">Тип данных</typeparam>
    internal sealed class XlatTable<T> where T : class
    {
        private readonly Dictionary<uint, T> _items;

        public XlatTable()
        {
            _items = new Dictionary<uint, T>();
        }

        public int Count => _items.Count;

        public void Add(uint refId, T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (_items.ContainsKey(refId))
                throw new ArchiveException($"Элемент с REFID {refId} уже существует");

            _items[refId] = item;
        }

        public bool TryGet(uint refId, out T item)
        {
            return _items.TryGetValue(refId, out item);
        }

        public T Get(uint refId)
        {
            if (!_items.TryGetValue(refId, out var item))
                throw new ArchiveException($"Элемент с REFID {refId} не найден");

            return item;
        }

        public bool Contains(uint refId)
        {
            return _items.ContainsKey(refId);
        }

        public bool Remove(uint refId)
        {
            return _items.Remove(refId);
        }

        public IEnumerable<uint> Keys => _items.Keys;
        public IEnumerable<T> Values => _items.Values;

        public void Clear()
        {
            _items.Clear();
        }
    }
}