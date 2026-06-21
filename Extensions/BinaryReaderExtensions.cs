using System;
using System.IO;
using System.Text;

namespace VitalEngine.Archive.Extensions
{
    /// <summary>
    /// Расширения для BinaryReader
    /// </summary>
    internal static class BinaryReaderExtensions
    {
        /// <summary>
        /// Чтение строки с null-терминатором (UTF-8)
        /// </summary>
        public static string ReadNullTerminatedString(this BinaryReader reader)
        {
            var bytes = new System.Collections.Generic.List<byte>();
            byte b;
            while ((b = reader.ReadByte()) != 0)
                bytes.Add(b);

            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        /// <summary>
        /// Чтение строки с null-терминатором (UTF-16)
        /// </summary>
        public static string ReadNullTerminatedStringUtf16(this BinaryReader reader)
        {
            var bytes = new System.Collections.Generic.List<byte>();
            byte b;
            while (true)
            {
                b = reader.ReadByte();
                if (b == 0)
                {
                    var next = reader.ReadByte();
                    if (next == 0)
                        break;
                    bytes.Add(b);
                    bytes.Add(next);
                    continue;
                }
                bytes.Add(b);
            }

            return Encoding.Unicode.GetString(bytes.ToArray());
        }

        /// <summary>
        /// Чтение строки фиксированной длины
        /// </summary>
        public static string ReadFixedString(this BinaryReader reader, int length)
        {
            var bytes = reader.ReadBytes(length);
            int nullIndex = Array.IndexOf(bytes, (byte)0);
            if (nullIndex >= 0)
                return Encoding.UTF8.GetString(bytes, 0, nullIndex);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Пропуск указанного количества байт
        /// </summary>
        public static void Skip(this BinaryReader reader, int bytes)
        {
            if (bytes < 0)
                throw new ArgumentException("Количество байт для пропуска не может быть отрицательным", nameof(bytes));

            reader.BaseStream.Seek(bytes, SeekOrigin.Current);
        }

        /// <summary>
        /// Проверка сигнатуры
        /// </summary>
        public static bool CheckSignature(this BinaryReader reader, byte[] signature)
        {
            var position = reader.BaseStream.Position;
            var bytes = reader.ReadBytes(signature.Length);
            reader.BaseStream.Seek(position, SeekOrigin.Begin);

            if (bytes.Length != signature.Length)
                return false;

            for (int i = 0; i < signature.Length; i++)
            {
                if (bytes[i] != signature[i])
                    return false;
            }
            return true;
        }
    }
}