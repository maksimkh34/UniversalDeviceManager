﻿using System.Diagnostics;

namespace UDM.Model.LogService
{
    public class LogEntry
    {
        public string Message { get; set; }

        public readonly DateTime? Date = DateTime.Now;
        public readonly LogLevel Level;
        public readonly string Invoker;

        public string GetReadable() => Level + ":\t" + Message;

        public LogEntry(string message, LogLevel level)
        {
            Message = message;
            Level = level;

            var frame = new StackTrace().GetFrames()[2];
            Invoker = frame.GetMethod()?.Name ?? "default";
        }
    }

    public enum LogLevel
    {
        Info,           // Информация о текущем выполнении программы (доступная для пользователя)
        Warning,        // Предупреждение о неожиданном результате в ходе выполнения, не препятствующем дальнейшему выполнению
        Error,          // Ошибка, не позволяющая завершить текущую операцию или процесс
        Fatal,          // Ошибка, не позволяющая продолжить выполнение программы ни при каких обстоятельствах
        Debug,          // Информация для отладки (разработчиков)
        NotSpecified    // Начальное значение
    }
}
