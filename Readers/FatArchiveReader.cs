using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VitalEngine.Archive.Exceptions;
using VitalEngine.Archive.Extensions;
using VitalEngine.Archive.Interfaces;
using VitalEngine.Archive.Models;

namespace VitalEngine.Archive.Readers
{
    /// <summary>
    /// Читает .fat файлы игры
    /// </summary>
    public sealed class FatArchiveReader : IArchiveReader
    {
        private const int HeaderOffset = 47;
        private const int SkipFolders = 14;
        private const int SkipFiles = 22;

        /// <summary>
        /// Синхронное чтение архива
        /// </summary>
        public ArchiveInfo ReadArchive(string fatPath)
        {
            if (string.IsNullOrEmpty(fatPath))
                throw new ArgumentException("Путь к .fat файлу не указан", nameof(fatPath));

            if (!File.Exists(fatPath))
                throw new ArchiveNotFoundException(fatPath);

            try
            {
                using (var stream = File.OpenRead(fatPath))
                using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
                {
                    return ReadArchiveInternal(reader, CancellationToken.None);
                }
            }
            catch (ArchiveException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ArchiveCorruptedException($"Ошибка чтения архива: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Асинхронное чтение архива
        /// </summary>
        public Task<ArchiveInfo> ReadArchiveAsync(
            string fatPath,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(fatPath))
                throw new ArgumentException("Путь к .fat файлу не указан", nameof(fatPath));

            if (!File.Exists(fatPath))
                throw new ArchiveNotFoundException(fatPath);

            try
            {
                using (var stream = File.OpenRead(fatPath))
                using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
                {
                    var result = ReadArchiveInternal(reader, cancellationToken);
                    return Task.FromResult(result);
                }
            }
            catch (ArchiveException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ArchiveCorruptedException($"Ошибка чтения архива: {ex.Message}", ex);
            }
        }

        private static ArchiveInfo ReadArchiveInternal(
            BinaryReader reader,
            CancellationToken cancellationToken)
        {
            // 1. Информационная строка
            var infoSize = reader.ReadInt32();
            var info = Encoding.UTF8.GetString(reader.ReadBytes(infoSize));

            // 2. Смещение к таблице архивов
            reader.Skip(HeaderOffset);

            // 3. Чтение архивных файлов (.grp)
            var containers = new List<ArchiveContainer>();
            var containerMap = new Dictionary<int, int>();

            var gfCount = reader.ReadInt16();
            for (int i = 0; i < gfCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var id = reader.ReadInt32();
                var nameSize = reader.ReadInt32();
                var name = Encoding.UTF8.GetString(reader.ReadBytes(nameSize));

                containers.Add(new ArchiveContainer
                {
                    Id = id,
                    Name = name
                });

                containerMap[id] = i;

                if (i == 0)
                    reader.Skip(2);
            }

            // 4. Пропускаем 14 байт
            reader.Skip(SkipFolders);

            // 5. Чтение папок
            var folders = new List<ArchiveFolder>();
            var folderMap = new Dictionary<int, int>();

            var foldersCount = reader.ReadInt16();
            for (int i = 0; i < foldersCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var id = reader.ReadInt32();
                var nameSize = reader.ReadInt32();
                var name = Encoding.UTF8.GetString(reader.ReadBytes(nameSize));

                folders.Add(new ArchiveFolder
                {
                    Id = id,
                    Path = name
                });

                folderMap[id] = i;

                if (i == 0)
                    reader.Skip(2);
            }

            // 6. Пропускаем 22 байта
            reader.Skip(SkipFiles);

            // 7. Чтение файлов
            var entries = new List<ArchiveEntry>();

            var filesCount = reader.ReadInt16();
            for (int i = 0; i < filesCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var id = reader.ReadInt32();
                var nameSize = reader.ReadInt32();
                var name = Encoding.UTF8.GetString(reader.ReadBytes(nameSize));

                var folderId = reader.ReadInt32();
                var archiveId = reader.ReadByte();
                var offset = reader.ReadUInt32();
                var size = reader.ReadUInt32();
                var unknown = reader.ReadInt64();

                string folderPath;
                if (folderMap.TryGetValue(folderId, out var folderIndex))
                    folderPath = folders[folderIndex].Path;
                else
                    folderPath = string.Empty;

                var fullPath = string.IsNullOrEmpty(folderPath)
                    ? name
                    : folderPath + name;

                entries.Add(new ArchiveEntry
                {
                    Id = id,
                    Name = name,
                    FolderId = folderId,
                    ArchiveId = archiveId,
                    Offset = offset,
                    Size = size,
                    Unknown = unknown,
                    FullPath = fullPath
                });

                if (i == 0)
                    reader.Skip(2);
            }

            return new ArchiveInfo
            {
                Info = info,
                Containers = containers,
                Folders = folders,
                Entries = entries
            };
        }
    }
}