# GFArchive

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-blueviolet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-8.0-green)](https://docs.microsoft.com/en-us/dotnet/csharp/)

Библиотека для распаковки, просмотра и модификации игровых архивов формата **.fat / .grp** (движок **VitalEngine**).

---

## 📦 Возможности

* ✅ Чтение и парсинг **main.fat**
* ✅ Извлечение всех файлов или выбранных элементов
* ✅ Поддержка **Boost.MultiIndex**
* ✅ Восстановленные хеш-функции
* ✅ Работа с **UTF-16**
* ✅ Поддержка **XlatTable**
* ✅ Асинхронный API (`async/await`)
* ✅ Отчёт о прогрессе
* ✅ Поддержка отмены через `CancellationToken`
* ✅ Интеграция в GUI, CLI и SDK

---

## 🧱 Архитектура

```text
VitalEngine.Archive
├── Models
├── Core
├── Readers
├── Extractors
├── Services
├── Interfaces
├── Exceptions
├── Extensions
└── Public
```

---

## 🚀 Быстрый старт

### Установка

```bash
dotnet add package GFArchive
```

или

```xml
<Reference Include="GFArchive">
    <HintPath>..\Libs\GFArchive.dll</HintPath>
</Reference>
```

### Распаковка архива

```csharp
var unpacker = new ArchiveUnpacker();
var progress = new ProgressReporter();

await unpacker.ExtractArchiveAsync(
    fatPath: @"D:\Games\Game\main.fat",
    outputDirectory: @"D:\Games\Game\Extracted",
    progressReporter: progress,
    cancellationToken: CancellationToken.None
);
```

---

## 📊 Получение информации

```csharp
var info = await unpacker.GetArchiveInfoAsync(@"D:\Games\Game\main.fat");

Console.WriteLine($"Файлов: {info.TotalFiles}");
Console.WriteLine($"Папок: {info.TotalFolders}");
Console.WriteLine($"Контейнеров: {info.TotalContainers}");
```

---

## 📂 Извлечение выбранных файлов

```csharp
var entries = info.Entries
    .Where(e => e.FullPath.StartsWith("LEV\\ISLAND_1\\"))
    .Take(10);

await unpacker.ExtractSelectedAsync(
    fatPath,
    outputDirectory,
    entries
);
```

---

## 🔧 Требования

* .NET Framework 4.8+
* C# 8.0+

---

## 📁 Структура файла архива

### main.fat

```text
[InfoSize: int]
[Info: string]
[gfCount: short]
[Containers...]
[folders: short]
[Folders...]
[files: short]
[Entries...]
```

### Запись файла

```text
id: int
nameSize: int
name: string
folderId: int
archiveId: byte
offset: uint
size: uint
unknown: long
```

---

## 🔬 Восстановленные алгоритмы

### Хеш-функции

| Функция           | Использование       |
| ----------------- | ------------------- |
| boost::hash_range | Строковые пути      |
| FNV-1a            | Составные ключи     |
| CombineHash       | Комбинированный хеш |

### Индексы Boost.MultiIndex

| Индекс        | Тип               |
| ------------- | ----------------- |
| FileId        | random_access     |
| FilePath      | hashed_non_unique |
| Folder + File | hashed            |

---

## 🤝 Contributing

1. Fork проекта
2. Создай ветку
3. Внеси изменения
4. Создай Pull Request

---

## 📄 Лицензия

MIT License

---

## ⚠️ Предупреждение

Данная библиотека создана исключительно в образовательных и исследовательских целях.
Не используйте её для нарушения авторских прав.
