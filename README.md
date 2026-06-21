# GFArchive

Библиотека для распаковки архивов игры на движке VitalEngine (.fat / .grp).
# Использование
using VitalEngine.Archive;

var unpacker = new ArchiveUnpacker();

// Получить информацию об архиве
var info = await unpacker.GetArchiveInfoAsync("main.fat");

Console.WriteLine($"Файлов: {info.TotalFiles}");
Console.WriteLine($"Папок: {info.TotalFolders}");

// Распаковать всё
await unpacker.ExtractArchiveAsync("main.fat", "output");

// Распаковать выбранные файлы
var entries = info.Entries.Take(10);
await unpacker.ExtractSelectedAsync("main.fat", "output", entries);
