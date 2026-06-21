using System.Threading;
using System.Threading.Tasks;
using VitalEngine.Archive.Models;

namespace VitalEngine.Archive.Interfaces
{
    /// <summary>
    /// Интерфейс для валидации архива
    /// </summary>
    public interface IArchiveValidator
    {
        /// <summary>
        /// Проверка целостности архива (синхронно)
        /// </summary>
        bool Validate(string fatPath);

        /// <summary>
        /// Проверка целостности архива (асинхронно)
        /// </summary>
        Task<bool> ValidateAsync(
            string fatPath,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Проверка наличия всех .grp файлов (синхронно)
        /// </summary>
        bool ValidateGrpFiles(
            ArchiveInfo archiveInfo,
            string sourceDirectory,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Проверка наличия всех .grp файлов (асинхронно)
        /// </summary>
        Task<bool> ValidateGrpFilesAsync(
            ArchiveInfo archiveInfo,
            string sourceDirectory,
            CancellationToken cancellationToken = default);
    }
}