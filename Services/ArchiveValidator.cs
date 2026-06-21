using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VitalEngine.Archive.Interfaces;
using VitalEngine.Archive.Models;

namespace VitalEngine.Archive.Services
{
    /// <summary>
    /// Реализация валидации архива
    /// </summary>
    public sealed class ArchiveValidator : IArchiveValidator
    {
        public bool Validate(string fatPath)
        {
            if (string.IsNullOrEmpty(fatPath))
                return false;

            if (!File.Exists(fatPath))
                return false;

            try
            {
                var fileInfo = new FileInfo(fatPath);
                if (fileInfo.Length == 0)
                    return false;

                using (var stream = File.OpenRead(fatPath))
                {
                    var buffer = new byte[4];
                    var read = stream.Read(buffer, 0, 4);
                    return read == 4 && buffer.Length > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ValidateAsync(
            string fatPath,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(fatPath))
                return false;

            if (!File.Exists(fatPath))
                return false;

            try
            {
                var fileInfo = new FileInfo(fatPath);
                if (fileInfo.Length == 0)
                    return false;

                using (var stream = File.OpenRead(fatPath))
                {
                    var buffer = new byte[4];
                    var read = await stream.ReadAsync(buffer, 0, 4, cancellationToken)
                        .ConfigureAwait(false);
                    return read == 4 && buffer.Length > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ValidateGrpFiles(
            ArchiveInfo archiveInfo,
            string sourceDirectory,
            CancellationToken cancellationToken = default)
        {
            if (archiveInfo == null)
                throw new ArgumentNullException(nameof(archiveInfo));

            if (string.IsNullOrEmpty(sourceDirectory))
                throw new ArgumentException("Путь к исходной папке не указан", nameof(sourceDirectory));

            if (archiveInfo.Containers == null || archiveInfo.Containers.Count == 0)
                return false;

            foreach (var container in archiveInfo.Containers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var containerPath = Path.Combine(sourceDirectory, container.Name);

                if (!File.Exists(containerPath))
                    return false;

                var fileInfo = new FileInfo(containerPath);
                if (fileInfo.Length == 0)
                    return false;
            }

            return true;
        }

        public async Task<bool> ValidateGrpFilesAsync(
            ArchiveInfo archiveInfo,
            string sourceDirectory,
            CancellationToken cancellationToken = default)
        {
            if (archiveInfo == null)
                throw new ArgumentNullException(nameof(archiveInfo));

            if (string.IsNullOrEmpty(sourceDirectory))
                throw new ArgumentException("Путь к исходной папке не указан", nameof(sourceDirectory));

            if (archiveInfo.Containers == null || archiveInfo.Containers.Count == 0)
                return false;

            foreach (var container in archiveInfo.Containers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var containerPath = Path.Combine(sourceDirectory, container.Name);

                if (!File.Exists(containerPath))
                    return false;

                var fileInfo = new FileInfo(containerPath);
                if (fileInfo.Length == 0)
                    return false;
            }

            return await Task.FromResult(true).ConfigureAwait(false);
        }
    }
}