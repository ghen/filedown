using System;

using Microsoft.Extensions.Logging;

namespace FileDownload.Extensions {

  #region [LoggerExtensions type definition]

  /// <summary>
  /// ILogger extension methods for common scenarios.
  /// </summary>
  public static class LoggerExtensions {

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Fatal(this ILogger logger, String message, params Object[] args)
      => logger?.LogCritical(message, args);

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Fatal(this ILogger logger, Exception exception, String message, params Object[] args)
      => logger?.LogCritical(exception, message, args);

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Fatal(this ILogger logger, EventId eventId, String message, params Object[] args)
      => logger?.LogCritical(eventId, message, args);

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Fatal(this ILogger logger, EventId eventId, Exception exception, String message, params Object[] args)
      => logger?.LogCritical(eventId, exception, message, args);

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Error(this ILogger logger, String message, params Object[] args)
      => logger?.LogError(message, args);

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Error(this ILogger logger, Exception exception, String message, params Object[] args)
      => logger?.LogError(exception, message, args);

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Error(this ILogger logger, EventId eventId, Exception exception, String message, params Object[] args)
      => logger?.LogError(eventId, exception, message, args);

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Error(this ILogger logger, EventId eventId, String message, params Object[] args)
      => logger?.LogError(eventId, message, args);

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Warn(this ILogger logger, EventId eventId, String message, params Object[] args)
      => logger?.LogWarning(eventId, message, args);

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Warn(this ILogger logger, EventId eventId, Exception exception, String message, params Object[] args)
      => logger?.LogWarning(eventId, exception, message, args);

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Warn(this ILogger logger, String message, params Object[] args)
      => logger?.LogWarning(message, args);

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Warn(this ILogger logger, Exception exception, String message, params Object[] args)
      => logger?.LogWarning(exception, message, args);

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Info(this ILogger logger, EventId eventId, String message, params Object[] args)
      => logger?.LogInformation(eventId, message, args);

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Info(this ILogger logger, Exception exception, String message, params Object[] args)
      => logger?.LogInformation(exception, message, args);

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Info(this ILogger logger, EventId eventId, Exception exception, String message, params Object[] args)
      => logger?.LogInformation(eventId, exception, message, args);

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Info(this ILogger logger, String message, params Object[] args)
      => logger?.LogInformation(message, args);

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Debug(this ILogger logger, EventId eventId, Exception exception, String message, params Object[] args)
      => logger?.LogDebug(eventId, exception, message, args);

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Debug(this ILogger logger, EventId eventId, String message, params Object[] args)
      => logger?.LogDebug(eventId, message, args);

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Debug(this ILogger logger, Exception exception, String message, params Object[] args)
      => logger?.LogDebug(exception, message, args);

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Debug(this ILogger logger, String message, params Object[] args)
      => logger?.LogDebug(message, args);

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Trace(this ILogger logger, String message, params Object[] args)
      => logger?.LogTrace(message, args);

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Trace(this ILogger logger, Exception exception, String message, params Object[] args)
      => logger?.LogTrace(exception, message, args);

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Trace(this ILogger logger, EventId eventId, String message, params Object[] args)
      => logger?.LogTrace(eventId, message, args);

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <seealso cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message. For example: "Firing '{eventId}' for '{eventSource}'...".</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Trace(this ILogger logger, EventId eventId, Exception exception, String message, params Object[] args)
      => logger?.LogTrace(eventId, exception, message, args);

  }

  #endregion [LoggerExtensions type definition]

}
