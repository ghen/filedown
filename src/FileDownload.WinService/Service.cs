using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

namespace FileDownload.WinService {

  #region [Service type definition]

  /// <summary>
  /// Integrates .NET Core application with Windows Services infrastructure.
  /// </summary>
  /// <remarks>
  /// See also:
  ///   https://github.com/aspnet/Hosting/blob/2a98db6a73512b8e36f55a1e6678461c34f4cc4d/samples/GenericHostSample/ServiceBaseLifetime.cs
  /// </remarks>
  [Obsolete("Switch to IHostBuilder.UseWindowsService (available in Microsoft.Extensions.Hosting.WindowsServices).")]
  internal sealed class Service : ServiceBase, IHostLifetime {

    #region Private members

    private readonly TaskCompletionSource<Object> _delayStart = new TaskCompletionSource<Object>();
    private readonly IHostApplicationLifetime _applicationLifetime;

    #endregion Private members

    #region Constructor and Initialization

    /// <summary>
    /// Creates a new <see cref="Service"/> instance.
    /// </summary>
    /// <param name="applicationLifetime">Hosted service (application) events notifier.</param>
    public Service(IHostApplicationLifetime applicationLifetime) {
      this._applicationLifetime = applicationLifetime
        ?? throw new ArgumentNullException(nameof(applicationLifetime));
    }

    #endregion Constructor and Initialization

    #region IHostLifetime implementation

    /// <summary>
    /// Starts applications.
    /// </summary>
    /// <param name="cancellationToken">(Optional) Cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Nothing.</returns>
    public Task WaitForStartAsync(CancellationToken cancellationToken = default) {
      cancellationToken.Register(() => this._delayStart.TrySetCanceled());
      this._applicationLifetime.ApplicationStopping.Register(this.Stop);

      // IMPORTANT: Run application on a separate (background) thread and return control to the hosting container ASAP
      new Thread(this.Run).Start();
      return this._delayStart.Task;
    }

    /// <summary>
    /// Terminates application.
    /// </summary>
    /// <param name="cancellationToken">(Optional) Cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Nothing.</returns>
    public Task StopAsync(CancellationToken cancellationToken = default) {
      this.Stop();
      return Task.CompletedTask;
    }

    #endregion IHostLifetime implementation

    #region ServiceBase overrides

    /// <summary>
    /// Called upon Windows Service stat-up.
    /// </summary>
    /// <param name="args">Command line arguments passed to the program.</param>
    protected override void OnStart(String[] args) {
      this._delayStart.TrySetResult(null);

      base.OnStart(args);
    }

    /// <summary>
    /// Called upun Windows Service shut-down.
    /// </summary>
    protected override void OnStop() {
      // NOTE: This method may be called multiple times by service Stop, ApplicationStopping, and StopAsync.
      //       Hence StopApplication(..) uses a CancellationTokenSource and prevents any recursion.
      this._applicationLifetime.StopApplication();

      base.OnStop();
    }

    #endregion ServiceBase overrides

    #region Internal Helpers

    /// <summary>
    /// Executes application code.
    /// </summary>
    private void Run() {
      try {
        // NOTE: Blocks current thread until service is stopped
        ServiceBase.Run(this);

        this._delayStart.TrySetException(
          new InvalidOperationException("Service has stopped without starting.")
        );
      } catch (Exception ex) {
        _delayStart.TrySetException(ex);
      }
    }

    #endregion Internal Helpers

  }

  #endregion [Service type definition]

}
