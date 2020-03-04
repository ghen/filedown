using System;
using System.Threading;

namespace FileDownload.Services {

  #region [IAppService interface definition]

  /// <summary>
  /// Describes service functionality.
  /// </summary>
  public interface IAppService : IDisposable {

    /// <summary>
    /// Service display name.
    /// </summary>
    String Name { get; }

    /// <summary>
    /// Executes service logic.
    /// </summary>
    /// <remarks>
    /// Service should always respect <paramref name="cancellationToken"/> and shutdown gracefully.
    /// In certain scenarios it is acceptable to throw <seealso cref="OperationCanceledException"/>.
    /// </remarks>
    /// <param name="cancellationToken">(Optional) Cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <exception cref="OperationCanceledException">
    /// Throws if <paramref name="cancellationToken"/> has been triggered.
    /// </exception>
    void Execute(CancellationToken cancellationToken = default);

  }

  #endregion [IAppService interface definition]

}
