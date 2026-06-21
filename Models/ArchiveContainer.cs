namespace VitalEngine.Archive.Models
{
    /// <summary>
    /// Контейнер архива (.grp файл)
    /// Соответствует структуре из игры
    /// </summary>
    public sealed class ArchiveContainer
    {
        /// <summary>ID контейнера (порядковый номер)</summary>
        public int Id { get; set; }

        /// <summary>Имя файла (.grp)</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Полный путь к файлу</summary>
        public string FullPath { get; set; } = string.Empty;

        /// <summary>Размер файла (байт)</summary>
        public long Size { get; set; }

        public override string ToString() => Name;
    }
}