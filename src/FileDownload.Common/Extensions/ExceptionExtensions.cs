using System;
using System.Collections.Generic;
using System.Linq;

namespace FileDownload.Extensions {

  #region [ExceptionExtensions type definition]

  /// <summary>
  /// Exception extensions and helpers.
  /// </summary>
  public static class ExceptionExtensions {

    #region ListErrors

    /// <summary>
    /// Lists all inner exceptions, including parent (e.g. the top one).
    /// </summary>
    /// <param name="ex">Exception to enumerate through.</param>
    /// <returns>Lists of all exceptions.</returns>
    public static IEnumerable<Exception> ListErrors(this Exception ex) {
      while (ex != null) {
        yield return ex;
        ex = ex.InnerException;
      }
    }

    #endregion ListErrors

    #region DetailedMessage

    /// <summary>
    /// Builds detailed error message as:
    ///    "Top error message. Inner error message 1. Inner error message 2..."
    /// </summary>
    /// <remarks>
    /// Returns empty string if <paramref name="ex"/> is <value>null</value>.
    /// </remarks>
    /// <param name="ex">Exception to parse.</param>
    /// <param name="separator">Separator between error messages.</param>
    /// <returns>Detailed error message.</returns>
    public static string DetailedMessage(this Exception ex, String separator = " ")
      => String.Join(separator ?? " ", ex?.ListErrors().Select(e => e.Message) ?? Enumerable.Empty<String>());

    #endregion DetailedMessage

  }

  #endregion [ExceptionExtensions type definition]

}
