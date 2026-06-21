using System.Collections.Generic;

namespace VitalEngine.Archive.Models
{
    /// <summary>
    /// Полная информация об архиве
    /// Результат чтения main.fat
    /// </summary>
    public sealed class ArchiveInfo
    {
        /// <summary>Информационная строка (serialization::archive)</summary>
        public string Info { get; set; } = string.Empty;

        /// <summary>Список контейнеров (.grp файлы)</summary>
        public IReadOnlyList<ArchiveContainer> Containers { get; set; }
            = new List<ArchiveContainer>();

        /// <summary>Список папок</summary>
        public IReadOnlyList<ArchiveFolder> Folders { get; set; }
            = new List<ArchiveFolder>();

        /// <summary>Список файлов</summary>
        public IReadOnlyList<ArchiveEntry> Entries { get; set; }
            = new List<ArchiveEntry>();

        /// <summary>Общее количество файлов</summary>
        public int TotalFiles => Entries?.Count ?? 0;

        /// <summary>Общее количество папок</summary>
        public int TotalFolders => Folders?.Count ?? 0;

        /// <summary>Общее количество контейнеров</summary>
        public int TotalContainers => Containers?.Count ?? 0;
    }
}