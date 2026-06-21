using System.Threading;
using System.Threading.Tasks;
using VitalEngine.Archive.Models;

namespace VitalEngine.Archive.Interfaces
{
    /// <summary>
    /// Интерфейс для чтения архива
    /// </summary>
    public interface IArchiveReader
    {
        /// <summary>
        /// Асинхронное чтение заголовка и метаданных .fat файла
        /// </summary>
        /// <param name="fatPath">Путь к main.fat</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Информация об архиве</returns>
        Task<ArchiveInfo> ReadArchiveAsync(
            string fatPath,
            CancellationToken cancellationToken = default);
    }
}