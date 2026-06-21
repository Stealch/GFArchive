namespace VitalEngine.Archive.Models
{
    /// <summary>
    /// Запись о файле в архиве
    /// Соответствует структуре FileInfo из игры
    /// </summary>
    public sealed class ArchiveEntry
    {
        /// <summary>Порядковый ID записи</summary>
        public int Id { get; set; }

        /// <summary>Имя файла</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>REFID папки (связь с ArchiveFolder.Id)</summary>
        public int FolderId { get; set; }

        /// <summary>REFID контейнера (связь с ArchiveContainer.Id)</summary>
        public int ArchiveId { get; set; }

        /// <summary>Смещение в .grp файле</summary>
        public uint Offset { get; set; }

        /// <summary>Размер файла в байтах</summary>
        public uint Size { get; set; }

        /// <summary>Неизвестное поле (возможно хеш/timestamp)</summary>
        public long Unknown { get; set; }

        /// <summary>Полный путь (вычисляется при загрузке)</summary>
        public string FullPath { get; set; } = string.Empty;

        public override string ToString() => FullPath;
    }
}