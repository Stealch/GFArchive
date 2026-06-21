using System;

namespace VitalEngine.Archive.Interfaces
{
    /// <summary>
    /// Интерфейс для отчёта о прогрессе
    /// </summary>
    public interface IProgressReporter
    {
        /// <summary>Текущий прогресс (0-100)</summary>
        int Progress { get; }

        /// <summary>Текущее состояние</summary>
        string Status { get; }

        /// <summary>Обновить прогресс</summary>
        void Report(int progress, string status);

        /// <summary>Начать операцию</summary>
        void Start(int totalItems, string operationName);

        /// <summary>Завершить операцию</summary>
        void Finish();

        /// <summary>Сообщить об ошибке</summary>
        void Error(string message, Exception ex = null);
    }
}