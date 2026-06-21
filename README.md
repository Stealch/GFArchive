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
