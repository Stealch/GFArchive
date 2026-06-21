namespace VitalEngine.Archive.Models
{
    /// <summary>
    /// Папка в архиве
    /// Соответствует структуре из игры
    /// </summary>
    public sealed class ArchiveFolder
    {
        /// <summary>Глобальный ID папки (REFID)</summary>
        public int Id { get; set; }

        /// <summary>Путь папки (относительный)</summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>Родительская папка (если есть)</summary>
        public int? ParentId { get; set; }

        public override string ToString() => Path;
    }
}