#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VitalEngine.Archive.Exceptions;
using VitalEngine.Archive.Extractors;
using VitalEngine.Archive.Interfaces;
using VitalEngine.Archive.Models;
using VitalEngine.Archive.Readers;
using VitalEngine.Archive.Services;

namespace VitalEngine.Archive
{
    /// <summary>
    /// Главный API для работы с архивами игры
    /// </summary>
    public sealed class ArchiveUnpacker : IDisposable
    {
        private readonly IArchiveReader _reader;
        private readonly IArchiveExtractor _extractor;
        private readonly IArchiveValidator _validator;
        private readonly IProgressReporter _progressReporter;
        private bool _disposed;

        /// <summary>
        /// Создаёт экземпляр распаковщика с настройками по умолчанию
        /// </summary>
        public ArchiveUnpacker() : this(null, null, null, null)
        {
        }

        /// <summary>
        /// Создаёт экземпляр распаковщика с пользовательскими сервисами
        /// </summary>
        public ArchiveUnpacker(
            IArchiveReader? reader = null,
            IArchiveExtractor? extractor = null,
            IArchiveValidator? validator = null,
            IProgressReporter? progressReporter = null)
        {
            _reader = reader ?? new FatArchiveReader();
            _extractor = extractor ?? new ArchiveExtractor();
            _validator = validator ?? new ArchiveValidator();
            _progressReporter = progressReporter ?? new ProgressReporter();
        }

        /// <summary>
        /// Получить информацию об архиве без распаковки (синхронно)
        /// </summary>
        public ArchiveInfo GetArchiveInfo(string fatPath)
        {
            if (string.IsNullOrEmpty(fatPath))
                throw new ArgumentException("Путь к .fat файлу не указан", nameof(fatPath));

            // Используем синхронный метод
            var reader = _reader as FatArchiveReader;
            if (reader != null)
            {
                return reader.ReadArchive(fatPath);
            }

            // Fallback через асинхронный метод
            return _reader.ReadArchiveAsync(fatPath, CancellationToken.None)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Получить информацию об архиве без распаковки (асинхронно)
        /// </summary>
        public async Task<ArchiveInfo> GetArchiveInfoAsync(
            string fatPath,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(fatPath))
                throw new ArgumentException("Путь к .fat файлу не указан", nameof(fatPath));

            return await _reader.ReadArchiveAsync(fatPath, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Проверить целостность архива
        /// </summary>
        public async Task<bool> ValidateArchiveAsync(
            string fatPath,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(fatPath))
                throw new ArgumentException("Путь к .fat файлу не указан", nameof(fatPath));

            return await _validator.ValidateAsync(fatPath, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Распаковать архив полностью (синхронно)
        /// </summary>
        public void ExtractArchive(
            string fatPath,
            string outputDirectory,
            IProgressReporter? progressReporter = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(fatPath))
                throw new ArgumentException("Путь к .fat файлу не указан", nameof(fatPath));

            if (string.IsNullOrEmpty(outputDirectory))
                throw new ArgumentException("Путь к выходной папке не указан", nameof(outputDirectory));

            var reporter = progressReporter ?? _progressReporter;
            var sourceDirectory = Path.GetDirectoryName(fatPath);

            if (string.IsNullOrEmpty(sourceDirectory))
                throw new ArchiveException("Не удалось определить папку с архивом");

            try
            {
                // Читаем информацию об архиве
                reporter.Start(3, "Чтение архива");
                var archiveInfo = GetArchiveInfo(fatPath);
                reporter.Report(33, $"Найдено файлов: {archiveInfo.TotalFiles}");

                // Валидация
                reporter.Report(66, "Проверка целостности");
                var isValid = _validator.ValidateGrpFiles(archiveInfo, sourceDirectory, cancellationToken);
                if (!isValid)
                {
                    throw new ArchiveCorruptedException("Один или несколько .grp файлов отсутствуют");
                }

                // Создаём выходную папку
                Directory.CreateDirectory(outputDirectory);

                // Извлечение
                reporter.Report(75, "Извлечение файлов");
                _extractor.ExtractAll(archiveInfo, sourceDirectory, outputDirectory, reporter, cancellationToken);

                reporter.Finish();
            }
            catch (OperationCanceledException)
            {
                reporter.Error("Операция отменена");
                throw;
            }
            catch (Exception ex)
            {
                reporter.Error($"Ошибка: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Распаковать архив полностью (асинхронно)
        /// </summary>
        public async Task ExtractArchiveAsync(
            string fatPath,
            string outputDirectory,
            IProgressReporter? progressReporter = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(fatPath))
                throw new ArgumentException("Путь к .fat файлу не указан", nameof(fatPath));

            if (string.IsNullOrEmpty(outputDirectory))
                throw new ArgumentException("Путь к выходной папке не указан", nameof(outputDirectory));

            var reporter = progressReporter ?? _progressReporter;
            var sourceDirectory = Path.GetDirectoryName(fatPath);

            if (string.IsNullOrEmpty(sourceDirectory))
                throw new ArchiveException("Не удалось определить папку с архивом");

            try
            {
                // Читаем информацию об архиве
                reporter.Start(3, "Чтение архива");
                var archiveInfo = await GetArchiveInfoAsync(fatPath, cancellationToken)
                    .ConfigureAwait(false);
                reporter.Report(33, $"Найдено файлов: {archiveInfo.TotalFiles}");

                // Валидация
                reporter.Report(66, "Проверка целостности");
                var isValid = await _validator.ValidateGrpFilesAsync(archiveInfo, sourceDirectory, cancellationToken)
                    .ConfigureAwait(false);
                if (!isValid)
                {
                    throw new ArchiveCorruptedException("Один или несколько .grp файлов отсутствуют");
                }

                // Создаём выходную папку
                Directory.CreateDirectory(outputDirectory);

                // Извлечение (синхронно внутри async метода)
                reporter.Report(75, "Извлечение файлов");
                await Task.Run(() => _extractor.ExtractAll(archiveInfo, sourceDirectory, outputDirectory, reporter, cancellationToken), cancellationToken)
                    .ConfigureAwait(false);

                reporter.Finish();
            }
            catch (OperationCanceledException)
            {
                reporter.Error("Операция отменена");
                throw;
            }
            catch (Exception ex)
            {
                reporter.Error($"Ошибка: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Распаковать выбранные файлы (синхронно)
        /// </summary>
        public void ExtractSelected(
            string fatPath,
            string outputDirectory,
            IEnumerable<ArchiveEntry> entries,
            IProgressReporter? progressReporter = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(fatPath))
                throw new ArgumentException("Путь к .fat файлу не указан", nameof(fatPath));

            if (string.IsNullOrEmpty(outputDirectory))
                throw new ArgumentException("Путь к выходной папке не указан", nameof(outputDirectory));

            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            var reporter = progressReporter ?? _progressReporter;
            var sourceDirectory = Path.GetDirectoryName(fatPath);

            if (string.IsNullOrEmpty(sourceDirectory))
                throw new ArchiveException("Не удалось определить папку с архивом");

            try
            {
                // Читаем информацию об архиве
                reporter.Start(2, "Подготовка");
                var archiveInfo = GetArchiveInfo(fatPath);
                reporter.Report(50, "Извлечение выбранных файлов");

                // Создаём выходную папку
                Directory.CreateDirectory(outputDirectory);

                _extractor.ExtractSelected(archiveInfo, sourceDirectory, outputDirectory, entries, reporter, cancellationToken);

                reporter.Finish();
            }
            catch (OperationCanceledException)
            {
                reporter.Error("Операция отменена");
                throw;
            }
            catch (Exception ex)
            {
                reporter.Error($"Ошибка: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Распаковать выбранные файлы (асинхронно)
        /// </summary>
        public async Task ExtractSelectedAsync(
            string fatPath,
            string outputDirectory,
            IEnumerable<ArchiveEntry> entries,
            IProgressReporter? progressReporter = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(fatPath))
                throw new ArgumentException("Путь к .fat файлу не указан", nameof(fatPath));

            if (string.IsNullOrEmpty(outputDirectory))
                throw new ArgumentException("Путь к выходной папке не указан", nameof(outputDirectory));

            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            var reporter = progressReporter ?? _progressReporter;
            var sourceDirectory = Path.GetDirectoryName(fatPath);

            if (string.IsNullOrEmpty(sourceDirectory))
                throw new ArchiveException("Не удалось определить папку с архивом");

            try
            {
                // Читаем информацию об архиве
                reporter.Start(2, "Подготовка");
                var archiveInfo = await GetArchiveInfoAsync(fatPath, cancellationToken)
                    .ConfigureAwait(false);
                reporter.Report(50, "Извлечение выбранных файлов");

                // Создаём выходную папку
                Directory.CreateDirectory(outputDirectory);

                await Task.Run(() => _extractor.ExtractSelected(archiveInfo, sourceDirectory, outputDirectory, entries, reporter, cancellationToken), cancellationToken)
                    .ConfigureAwait(false);

                reporter.Finish();
            }
            catch (OperationCanceledException)
            {
                reporter.Error("Операция отменена");
                throw;
            }
            catch (Exception ex)
            {
                reporter.Error($"Ошибка: {ex.Message}", ex);
                throw;
            }
        }

        #region IDisposable

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_extractor is IDisposable disposable)
                    disposable.Dispose();

                if (_reader is IDisposable readerDisposable)
                    readerDisposable.Dispose();

                _disposed = true;
            }
        }

        #endregion
    }
}