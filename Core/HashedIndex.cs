using System;
using System.Collections.Generic;
using VitalEngine.Archive.Exceptions;

namespace VitalEngine.Archive.Core
{
    /// <summary>
    /// Хеш-индекс с открытой адресацией
    /// Соответствует Boost.MultiIndex hashed_index
    /// </summary>
    internal sealed class HashedIndex<T> where T : class
    {
        private readonly T[] _buckets;
        private readonly byte[] _states;
        private readonly uint _mask;
        private uint _firstNonEmpty;
        private readonly bool _unique;

        private enum State : byte
        {
            Empty = 0,
            Occupied = 1,
            Deleted = 2
        }

        public HashedIndex(int bucketSize, bool unique = true)
        {
            if (bucketSize <= 0)
                throw new ArgumentException("Размер должен быть больше 0", nameof(bucketSize));

            bucketSize = (int)NextPowerOfTwo((uint)bucketSize);

            _buckets = new T[bucketSize];
            _states = new byte[bucketSize];
            _mask = (uint)(bucketSize - 1);
            _firstNonEmpty = (uint)bucketSize;
            _unique = unique;
        }

        public int Count { get; private set; }
        public int Capacity => _buckets.Length;

        public bool Insert(uint key, T value)
        {
            uint index = VitalHash.HashId(key) & _mask;
            uint start = index;

            while (_states[index] != (byte)State.Empty)
            {
                if (_states[index] == (byte)State.Occupied && _unique)
                {
                    return false;
                }

                index = (index + 1) & _mask;
                if (index == start)
                    throw new ArchiveException("Хеш-таблица заполнена");
            }

            _buckets[index] = value;
            _states[index] = (byte)State.Occupied;

            if (index < _firstNonEmpty)
                _firstNonEmpty = index;

            Count++;
            return true;
        }

        public bool Remove(uint key)
        {
            uint index = VitalHash.HashId(key) & _mask;
            uint start = index;

            while (_states[index] != (byte)State.Empty)
            {
                if (_states[index] == (byte)State.Occupied)
                {
                    _states[index] = (byte)State.Deleted;
                    Count--;

                    if (index == _firstNonEmpty)
                    {
                        while (_states[_firstNonEmpty] != (byte)State.Occupied)
                        {
                            _firstNonEmpty = (_firstNonEmpty + 1) & _mask;
                        }
                    }

                    return true;
                }

                index = (index + 1) & _mask;
                if (index == start)
                    break;
            }

            return false;
        }

        public bool TryGet(uint key, out T value)
        {
            uint index = VitalHash.HashId(key) & _mask;
            uint start = index;

            while (_states[index] != (byte)State.Empty)
            {
                if (_states[index] == (byte)State.Occupied)
                {
                    value = _buckets[index];
                    return true;
                }

                index = (index + 1) & _mask;
                if (index == start)
                    break;
            }

            value = null;
            return false;
        }

        private static uint NextPowerOfTwo(uint value)
        {
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value++;
            return value;
        }
    }
}