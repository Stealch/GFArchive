using System;

namespace VitalEngine.Archive.Exceptions
{
    /// <summary>
    /// Базовое исключение для всех ошибок архива
    /// </summary>
    public class ArchiveException : Exception
    {
        public ArchiveException()
            : base()
        {
        }

        public ArchiveException(string message)
            : base(message)
        {
        }

        public ArchiveException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        // Для сериализации
        protected ArchiveException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Исключение: файл архива не найден
    /// </summary>
    public class ArchiveNotFoundException : ArchiveException
    {
        public ArchiveNotFoundException()
            : base()
        {
        }

        public ArchiveNotFoundException(string path)
            : base($"Архив не найден: {path}")
        {
        }

        public ArchiveNotFoundException(string path, Exception innerException)
            : base($"Архив не найден: {path}", innerException)
        {
        }

        protected ArchiveNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Исключение: архив повреждён
    /// </summary>
    public class ArchiveCorruptedException : ArchiveException
    {
        public ArchiveCorruptedException()
            : base()
        {
        }

        public ArchiveCorruptedException(string message)
            : base(message)
        {
        }

        public ArchiveCorruptedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ArchiveCorruptedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Исключение: нет доступа к архиву
    /// </summary>
    public class ArchiveAccessDeniedException : ArchiveException
    {
        public ArchiveAccessDeniedException()
            : base()
        {
        }

        public ArchiveAccessDeniedException(string path)
            : base($"Нет доступа к архиву: {path}")
        {
        }

        public ArchiveAccessDeniedException(string path, Exception innerException)
            : base($"Нет доступа к архиву: {path}", innerException)
        {
        }

        protected ArchiveAccessDeniedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}