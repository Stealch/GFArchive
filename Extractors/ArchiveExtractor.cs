#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VitalEngine.Archive.Exceptions;
using VitalEngine.Archive.Interfaces;
using VitalEngine.Archive.Models;

namespace VitalEngine.Archive.Extractors
{
    /// <summary>
    /// Извлекает файлы из архива
    /// </summary>
    public sealed class ArchiveExtractor : IArchiveExtractor
    {
        private const int BufferSize = 81920; // 80KB

        public void ExtractAll(
            ArchiveInfo archiveInfo,
            string sourceDirectory,
            string outputDirectory,
            IProgressReporter? progressReporter = null,
            CancellationToken cancellationToken = default)
        {
            if (archiveInfo == null)
                throw new ArgumentNullException(nameof(archiveInfo));

            if (string.IsNullOrEmpty(sourceDirectory))
                throw new ArgumentException("Путь к исходной папке не указан", nameof(sourceDirectory));

            if (string.IsNullOrEmpty(outputDirectory))
                throw new ArgumentException("Путь к выходной папке не указан", nameof(outputDirectory));

            // Группируем файлы по контейнерам
            var groupedByArchive = archiveInfo.Entries
                .GroupBy(e => e.ArchiveId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var totalFiles = archiveInfo.TotalFiles;
            var processedFiles = 0;

            progressReporter?.Start(totalFiles, "Извлечение файлов");

            foreach (var container in archiveInfo.Containers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!groupedByArchive.TryGetValue(container.Id, out var entries))
                    continue;

                var containerPath = Path.Combine(sourceDirectory, container.Name);

                if (!File.Exists(containerPath))
                {
                    progressReporter?.Error($"Архив не найден: {container.Name}");
                    throw new ArchiveNotFoundException(containerPath);
                }

                using (var containerStream = File.OpenRead(containerPath))
                {
                    var buffer = new byte[BufferSize];

                    foreach (var entry in entries)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var outputPath = Path.Combine(outputDirectory, entry.FullPath);
                        var outputDir = Path.GetDirectoryName(outputPath);

                        if (!string.IsNullOrEmpty(outputDir))
                            Directory.CreateDirectory(outputDir);

                        ExtractFile(
                            containerStream,
                            entry,
                            outputPath,
                            buffer);

                        processedFiles++;
                        var progress = (int)((double)processedFiles / totalFiles * 100);
                        progressReporter?.Report(progress, $"{entry.FullPath} ({processedFiles}/{totalFiles})");
                    }
                }
            }

            progressReporter?.Finish();
        }

        public void ExtractSelected(
            ArchiveInfo archiveInfo,
            string sourceDirectory,
            string outputDirectory,
            IEnumerable<ArchiveEntry> entries,
            IProgressReporter? progressReporter = null,
            CancellationToken cancellationToken = default)
        {
            if (archiveInfo == null)
                throw new ArgumentNullException(nameof(archiveInfo));

            if (string.IsNullOrEmpty(sourceDirectory))
                throw new ArgumentException("Путь к исходной папке не указан", nameof(sourceDirectory));

            if (string.IsNullOrEmpty(outputDirectory))
                throw new ArgumentException("Путь к выходной папке не указан", nameof(outputDirectory));

            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            var entryList = entries.ToList();
            if (entryList.Count == 0)
                return;

            // Группируем по контейнерам
            var groupedByArchive = entryList
                .GroupBy(e => e.ArchiveId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var totalFiles = entryList.Count;
            var processedFiles = 0;

            progressReporter?.Start(totalFiles, "Извлечение выбранных файлов");

            // Кэш контейнеров
            var containerMap = archiveInfo.Containers
                .ToDictionary(c => c.Id, c => c.Name);

            foreach (var group in groupedByArchive)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var archiveId = group.Key;
                var entriesInContainer = group.Value;

                if (!containerMap.TryGetValue(archiveId, out var containerName))
                {
                    progressReporter?.Error($"Контейнер {archiveId} не найден");
                    continue;
                }

                var containerPath = Path.Combine(sourceDirectory, containerName);

                if (!File.Exists(containerPath))
                {
                    progressReporter?.Error($"Архив не найден: {containerName}");
                    continue;
                }

                using (var containerStream = File.OpenRead(containerPath))
                {
                    var buffer = new byte[BufferSize];

                    foreach (var entry in entriesInContainer)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var outputPath = Path.Combine(outputDirectory, entry.FullPath);
                        var outputDir = Path.GetDirectoryName(outputPath);

                        if (!string.IsNullOrEmpty(outputDir))
                            Directory.CreateDirectory(outputDir);

                        ExtractFile(
                            containerStream,
                            entry,
                            outputPath,
                            buffer);

                        processedFiles++;
                        var progress = (int)((double)processedFiles / totalFiles * 100);
                        progressReporter?.Report(progress, $"{entry.FullPath} ({processedFiles}/{totalFiles})");
                    }
                }
            }

            progressReporter?.Finish();
        }

        private static void ExtractFile(
            FileStream containerStream,
            ArchiveEntry entry,
            string outputPath,
            byte[] buffer)
        {
            containerStream.Seek(entry.Offset, SeekOrigin.Begin);

            using (var outputStream = File.Create(outputPath))
            {
                long remaining = entry.Size;
                while (remaining > 0)
                {
                    var toRead = (int)Math.Min(remaining, buffer.Length);
                    var read = containerStream.Read(buffer, 0, toRead);

                    if (read == 0)
                        break;

                    outputStream.Write(buffer, 0, read);
                    remaining -= read;
                }
            }
        }
    }
}