# GFArchive

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-blueviolet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-8.0-green)](https://docs.microsoft.com/en-us/dotnet/csharp/)

Библиотека для распаковки, просмотра и модификации игровых архивов формата **.fat / .grp** (движок **VitalEngine**).

---

## 📦 Возможности

- ✅ Чтение и парсинг **main.fat** — полностью воссозданная структура через реверс-инжиниринг
- ✅ Извлечение всех файлов или выбранных элементов
- ✅ Поддержка **Boost.MultiIndex** (хеш-индексы по ID, пути и составному ключу)
- ✅ Точное восстановление хеш-функций (`boost::hash_range`, FNV-1a)
- ✅ Работа с **UTF-16** строками (как в оригинальной игре)
- ✅ Обработка **XlatTable** (трансляция REFID → данные)
- ✅ Асинхронный API (`async`/`await`)
- ✅ Отчёт о прогрессе через `IProgressReporter`
- ✅ Поддержка отмены через `CancellationToken`
- ✅ Готов к интеграции в GUI, CLI или SDK

---

## 🧱 Архитектура

Библиотека построена на основе данных, полученных в ходе **реверс-инжиниринга** отладочной версии игры с PDB-символами:

```text
VitalEngine.Archive
├── Models             # Data-модели (ArchiveInfo, ArchiveEntry...)
├── Core               # Хеш-функции, XlatTable, HashedIndex
├── Readers            # Чтение main.fat (FatArchiveReader)
├── Extractors         # Извлечение файлов из .grp
├── Services           # Валидация, прогресс
├── Interfaces         # Контракты (IProgressReporter, ...)
├── Exceptions         # Специфичные исключения
├── Extensions         # BinaryReaderExtensions
└── Public             # ArchiveUnpacker — единый входной API
🚀 Быстрый старт
Установка
bash
dotnet add package GFArchive
Или добавьте ссылку на GFArchive.dll в ваш проект:

xml
<Reference Include="GFArchive">
    <HintPath>..\Libs\GFArchive.dll</HintPath>
</Reference>
Пример использования
csharp
using VitalEngine.Archive;
using VitalEngine.Archive.Interfaces;
using VitalEngine.Archive.Services;
using System.Threading;
using System.Threading.Tasks;

public async Task ExtractGameArchiveAsync()
{
    var unpacker = new ArchiveUnpacker();
    var progress = new ProgressReporter();

    progress.ProgressChanged += (s, e) =>
        Console.WriteLine($"[{e.Progress}%] {e.Status}");

    var cts = new CancellationTokenSource();

    await unpacker.ExtractArchiveAsync(
        fatPath: @"D:\Games\Game\main.fat",
        outputDirectory: @"D:\Games\Game\Extracted",
        progressReporter: progress,
        cancellationToken: cts.Token
    );

    Console.WriteLine("✅ Распаковка завершена!");
}
## Получение информации об архиве
csharp
var info = await unpacker.GetArchiveInfoAsync(@"D:\Games\Game\main.fat");

Console.WriteLine($"Файлов: {info.TotalFiles}");
Console.WriteLine($"Папок: {info.TotalFolders}");
Console.WriteLine($"Контейнеров (.grp): {info.TotalContainers}");
Извлечение выбранных файлов
csharp
var info = await unpacker.GetArchiveInfoAsync(fatPath);

var entries = info.Entries
    .Where(e => e.FullPath.StartsWith("LEV\\ISLAND_1\\"))
    .Take(10);

await unpacker.ExtractSelectedAsync(
    fatPath,
    outputDirectory,
    entries
);
## 🔧 Требования
.NET Framework 4.8 или выше

C# 8.0 (или выше для nullable-аннотаций, если включены)

## 🧪 Тестирование
Проект покрыт модульными тестами (NUnit):

bash
dotnet test
📁 Структура файла архива
main.fat (Boost.Serialization)
text
[InfoSize: int]         # 22
[Info: string]          # "serialization::archive"
[gfCount: short]        # Количество .grp
[Containers...]         # XlatTable<ArchiveContainer>
[folders: short]        # Количество папок
[Folders...]            # XlatTable<ArchiveFolder>
[files: short]          # Количество файлов
[Entries...]            # XlatTable<ArchiveEntry>
Каждая запись файла
text
id: int                 # Порядковый ID
nameSize: int
name: string            # Имя файла
folderId: int           # REFID папки
archiveId: byte         # ID .grp контейнера
offset: uint            # Смещение внутри .grp
size: uint              # Размер файла
unknown: long           # Хеш/timestamp (сохраняется как есть)
## 🔬 Восстановленные алгоритмы
Хеш-функции
Функция	Использование	Алгоритм
boost::hash_range	Строковые пути	hash ^= hash * 0x40 + (hash >> 2) + 0x9E3779B9 + char
FNV-1a	Составные ключи	hash = hash * 0x1000193 ^ char
CombineHash	Комбинированный хеш	(h1>>3) + 0x9E3779B9 + h1 ... ^ h1
Индексы (Boost.MultiIndex)
Индекс	Тип	Ключ
FileId	random_access	Числовой ID
FilePath	hashed_non_unique	Хеш полного пути
(FolderName, FileName)	hashed	Составной хеш
## 🤝 Contributing
Fork проекта

Создай ветку (git checkout -b feature/AmazingFeature)

Внеси изменения

Запушь (git push origin feature/AmazingFeature)

Открой Pull Request

## 📄 Лицензия
MIT © 2024

## 🙏 Благодарности
Boost — за MultiIndex и Serialization

QuickBMS — за оригинальный скрипт-образец

Сообщество — за поддержку и интерес к наследию студии

## 📬 Контакты
Issues: GitHub Issues

Discord: [Ссылка на сервер]

## ⚠️ Предупреждение
Данная библиотека создана исключительно в образовательных и исследовательских целях. Не используйте её для нарушения авторских прав или распространения контента, защищённого законом.
