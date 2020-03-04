using System;
using System.Threading;

using FileDownload.Generics;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FileDownload.Services {

  #region [AppService type definition]

  /// <summary>
  /// Basic service functionality.
  /// </summary>
  // NOTE: We DO know about Microsoft.Extensions.Hosting.Abstractions/BackgroundService class,
  //       and we DO NOT want to inherit from it due to garbage IHostedService implementation.
  // TODO: Extract IHostedService support into separate adapter class.
  public abstract class AppService<TSettings> : Disposable, IAppService
    where TSettings : AppServiceSettings {

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="AppService"/> instance.
    /// </summary>
    /// <param name="serviceProvider">Service provider.</param>
    /// <param name="settings">Service execution settings.</param>
    /// <param name="logger">(Optional) Logger to send disagnostic messages to.</param>
    protected AppService(IServiceProvider serviceProvider, TSettings settings, ILogger<AppService<TSettings>> logger = null) {
      this.ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      this.Config = settings ?? throw new ArgumentNullException(nameof(settings));
      this.Log = logger ?? NullLogger<AppService<TSettings>>.Instance;
    }

    #endregion Constructor and Initialization

    #region Properties

    /// <summary>
    /// Service configuration.
    /// </summary>
    public TSettings Config { get; }

    /// <summary>
    /// Service provider.
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Logger to send disagnostic messages to.
    /// </summary>
    protected ILogger<AppService<TSettings>> Log { get; }

    #endregion Properties

    #region IAppService implementation

    /// <inheritdoc/>
    public virtual String Name
      => String.IsNullOrEmpty(this.Config.Name) ? this.GetType().Name : this.Config.Name;

    /// <inheritdoc/>
    public abstract void Execute(CancellationToken cancellationToken = default);

    #endregion IAppService implementation

    #region ToString

    /// <inheritdoc/>
    public override String ToString()
      => this.Name;

    #endregion ToString

  }

  #endregion [AppService type definition]

}
