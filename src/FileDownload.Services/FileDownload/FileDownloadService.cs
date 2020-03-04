using System;
using System.Threading;

using FileDownload.Extensions;

using Microsoft.Extensions.Logging;

namespace FileDownload.Services {

  #region [FileDownloadService type definition]

  /// <summary>
  /// Processes scheduled <seealso cref="Data.Job"/>(s) asynchronously.
  /// </summary>
  public sealed class FileDownloadService : AppService<FileDownloadServiceSettings> {

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="FileDownloadService"/> instance.
    /// </summary>
    /// <param name="serviceProvider">Service provider.</param>
    /// <param name="settings">Service execution settings.</param>
    /// <param name="logger">(Optional) Logger to send disagnostic messages to.</param>
    public FileDownloadService(IServiceProvider serviceProvider, FileDownloadServiceSettings settings, ILogger<FileDownloadService> logger = null)
      : base(serviceProvider, settings, logger) { }

    #endregion Constructor and Initialization

    #region AppService overrides

    /// <inheritdoc/>
    public override void Execute(CancellationToken cancellationToken = default) {

      Log.Debug($"Doing something...");

      /*
      using (var serviceProvider = this.ServiceProvider.BeginLifetimeScope()) {
        this.DownloadChannelAdvisorOrders(serviceProvider, stores, cancellationToken: cancellationToken);
      }
      */

      Log.Debug($"Done.");
    }

    #endregion AppService overrides

  }

  #endregion [FileDownloadService type definition]

}
