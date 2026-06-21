using System;

namespace VitalEngine.Archive.Core
{
    /// <summary>
    /// Хеш-функции, используемые в игре
    /// Восстановлены через реверс-инжиниринг
    /// </summary>
    public static class VitalHash
    {
        private const uint FnvOffsetBasis = 0x811c9dc5;
        private const uint FnvPrime = 0x1000193;
        private const uint GoldenRatio = 0x9E3779B9;

        /// <summary>
        /// Хеш-функция Boost.hash_range (используется для путей)
        /// </summary>
        public static uint HashRange(string str)
        {
            if (string.IsNullOrEmpty(str))
                return 0;

            uint hash = 0;
            foreach (char c in str)
            {
                hash ^= (hash * 0x40 + (hash >> 2) + GoldenRatio + (uint)c);
            }
            return hash;
        }

        /// <summary>
        /// Хеш-функция FNV-1a (используется для составных ключей)
        /// </summary>
        public static uint HashRangeFNV(string str)
        {
            if (string.IsNullOrEmpty(str))
                return FnvOffsetBasis;

            uint hash = FnvOffsetBasis;
            foreach (char c in str)
            {
                hash = hash * FnvPrime ^ (uint)c;
            }
            return hash;
        }

        /// <summary>
        /// Комбинированный хеш (используется в unchecked_rehash)
        /// </summary>
        public static uint CombineHash(uint hash1, uint hash2)
        {
            uint temp = (hash1 >> 3) + GoldenRatio + hash1;
            return hash2 + GoldenRatio + temp * 0x40 + (temp >> 2) ^ temp;
        }

        /// <summary>
        /// Хеш для числовых ID (золотое сечение)
        /// </summary>
        public static uint HashId(uint id)
        {
            return id * GoldenRatio;
        }
    }
}