using VitalEngine.Archive.Core;

namespace VitalEngine.Archive.Models.Internal
{
    /// <summary>
    /// Внутренняя структура FileInfo из игры
    /// Используется в HashedIndex
    /// </summary>
    internal sealed class FileInfoInternal
    {
        public uint FileId { get; set; }
        public string FolderName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public uint ArchiveId { get; set; }
        public uint Offset { get; set; }
        public uint Size { get; set; }
        public long Unknown { get; set; }

        public string FullPath =>
            string.IsNullOrEmpty(FolderName)
                ? FileName
                : System.IO.Path.Combine(FolderName, FileName);

        /// <summary>
        /// Хеш для индекса по пути (использует boost::hash_range)
        /// </summary>
        public uint PathHash => VitalHash.HashRange(FullPath);

        /// <summary>
        /// Хеш для составного индекса (папка + имя)
        /// </summary>
        public uint CompositeHash => VitalHash.CombineHash(
            VitalHash.HashRangeFNV(FolderName),
            VitalHash.HashRangeFNV(FileName)
        );

        /// <summary>
        /// Хеш для ID (золотое сечение)
        /// </summary>
        public uint IdHash => VitalHash.HashId(FileId);
    }
}