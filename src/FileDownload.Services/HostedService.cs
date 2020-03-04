using System;
using System.Threading;
using System.Threading.Tasks;

using FileDownload.Extensions;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FileDownload.Services {

  #region [HostedService type definition]

  /// <summary>
  /// Wraps <seealso cref="IShimService"/> to be used as <seealso cref="IHostedService"/>.
  /// </summary>
  public sealed class HostedService<TService> : BackgroundService, IHostedService
    where TService : class, IAppService {

    #region Constructor and Initialization

    /// <summary>
    /// constructs new <see cref="HostedService"/> instance.
    /// </summary>
    /// <param name="service">Service instance to wrap.</param>
    /// <param name="logger">(Optional) Logger to send disagnostic messages to.</param>
    public HostedService(TService service, ILogger<TService> logger = null) {
      this.Service = service ?? throw new ArgumentNullException(nameof(service));
      this.Log = logger ?? new NullLogger<TService>();
    }

    #endregion Constructor and Initialization

    #region Properties

    /// <summary>
    /// Wrapped service instance.
    /// </summary>
    private TService Service { get; }

    /// <summary>
    /// Logger to send disagnostic messages to.
    /// </summary>
    private ILogger<TService> Log { get; }

    #endregion Properties

    #region BackgroundService overrides

    /// <inheritdoc/>
    public override Task StartAsync(CancellationToken cancellationToken) {
      Log.Info("{service} is starting...", this.Service.Name);
      return base.StartAsync(cancellationToken);
    }

    /// <inheritdoc/>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
      // IMPORTANT: We cann't simply return blocking Task from this method, as .NET Core 2.2 engine
      //            starts all hosted services in one thread sequentially. Thus the very first
      //            blocking Task would prevent all other services from being executed :(. Thank you MS!
      => Task.Run(() => {
        try {

          //
          // Execute service logic
          //
          this.Service.Execute(stoppingToken);

        } catch (Exception ex) {
          if (ex.GetType() == typeof(OperationCanceledException) && stoppingToken.IsCancellationRequested)
            throw;

          // IMPORTANT: .NET Core 2.2 implementation of BackgroundService does not report Task errors in any way.
          //            And running service on a thread pool thread simply swallows all errors.
          //            As we want to get error in logs as soon as they happen (usually on service startup),
          //            We catch an log those here.
          Log.Error(ex, "{service} execution failed with error.", this.Service.Name);
        }
      }, cancellationToken: stoppingToken);

    /// <inheritdoc/>
    public override Task StopAsync(CancellationToken cancellationToken)
      => base.StopAsync(cancellationToken)
             .ContinueWith(_ =>
                Log.Debug("{service} has stopped.", this.Service.Name
              ), continuationOptions: TaskContinuationOptions.OnlyOnRanToCompletion);

    #endregion BackgroundService overrides

  }

  #endregion [HostedService type definition]

}
