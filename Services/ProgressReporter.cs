#nullable enable

using System;
using VitalEngine.Archive.Interfaces;

namespace VitalEngine.Archive.Services
{
    /// <summary>
    /// Стандартная реализация репортёра прогресса
    /// </summary>
    public sealed class ProgressReporter : IProgressReporter
    {
        public int Progress { get; private set; }
        public string Status { get; private set; } = string.Empty;

        public event EventHandler<ProgressEventArgs>? ProgressChanged;
        public event EventHandler<ErrorEventArgs>? ErrorOccurred;
        public event EventHandler? Started;
        public event EventHandler? Finished;

        public void Report(int progress, string status)
        {
            // Math.Clamp недоступен в .NET Framework 4.8
            Progress = progress < 0 ? 0 : (progress > 100 ? 100 : progress);
            Status = status;
            ProgressChanged?.Invoke(this, new ProgressEventArgs(Progress, Status));
        }

        public void Start(int totalItems, string operationName)
        {
            Progress = 0;
            Status = $"Начало: {operationName} (0/{totalItems})";
            Started?.Invoke(this, EventArgs.Empty);
        }

        public void Finish()
        {
            Progress = 100;
            Status = "Завершено";
            Finished?.Invoke(this, EventArgs.Empty);
        }

        public void Error(string message, Exception? ex = null)
        {
            ErrorOccurred?.Invoke(this, new ErrorEventArgs(message, ex));
        }
    }

    public sealed class ProgressEventArgs : EventArgs
    {
        public int Progress { get; }
        public string Status { get; }

        public ProgressEventArgs(int progress, string status)
        {
            Progress = progress;
            Status = status;
        }
    }

    public sealed class ErrorEventArgs : EventArgs
    {
        public string Message { get; }
        public Exception? Exception { get; }

        public ErrorEventArgs(string message, Exception? exception = null)
        {
            Message = message;
            Exception = exception;
        }
    }
}