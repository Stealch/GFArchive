#nullable enable

using System.Collections.Generic;
using System.Threading;
using VitalEngine.Archive.Models;

namespace VitalEngine.Archive.Interfaces
{
    /// <summary>
    /// Интерфейс для извлечения файлов из архива
    /// </summary>
    public interface IArchiveExtractor
    {
        /// <summary>
        /// Извлечение всех файлов
        /// </summary>
        void ExtractAll(
            ArchiveInfo archiveInfo,
            string sourceDirectory,
            string outputDirectory,
            IProgressReporter? progressReporter = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Извлечение выбранных файлов
        /// </summary>
        void ExtractSelected(
            ArchiveInfo archiveInfo,
            string sourceDirectory,
            string outputDirectory,
            IEnumerable<ArchiveEntry> entries,
            IProgressReporter? progressReporter = null,
            CancellationToken cancellationToken = default);
    }
}