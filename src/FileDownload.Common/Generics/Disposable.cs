using System;

namespace FileDownload.Generics {

  #region [Disposable type definition]

  /// <summary>
  /// Implements standard routines for disposable pattern support.
  /// </summary>
  /// <remarks>
  /// See <seealso cref="IDisposable"/> interface description in MSDN for more information.
  /// </remarks>
  public abstract class Disposable : IDisposable {

    #region IDisposable implementation

    private readonly Object _sysRoot = new Object();

    /// <summary>
    /// We override destructor to control object finalization process.
    /// </summary>
    ~Disposable() {
      this.InternalDispose(false);
    }

    /// <summary>
    /// This method normally should be called to release the object.
    /// </summary>
    /// <remarks>
    /// This method is thread-safe.
    /// </remarks>
    public void Dispose() {
      if (!this.IsDisposed) {
        this.InternalDispose(true);
        this.IsDisposed = true;
      }

      // Tell GC to skip finallization as we released the object already
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Internal disposal procedure that makes Dispose methos thread-safe.
    /// </summary>
    /// <param name="disposing">Indicates if this method is called by user ('true') or by GC during finalization ('false').</param>
    private void InternalDispose(Boolean disposing) {

      if (!this.IsDisposed) {

        // IMPORTANT: We should not lock the thread during finalization process!
        //            GC will never call destructor from multiple threads.
        if (!disposing) {
          this.OnDisposing(disposing);
        } else {
          lock (this._sysRoot) {
            if (!this.IsDisposed)
              this.OnDisposing(disposing);
          }
        }

        this.IsDisposed = true;
      }

    }

    /// <summary>
    /// Releases object resources.
    /// If <paramref name="disposedByUser"/> is set to <value>false</value>, only unmanaged resources should be released.
    /// </summary>
    /// <remarks>
    /// Overrided this method in nested classes.
    /// Use the implementation below as a template.
    /// </remarks>
    /// <param name="disposedByUser">
    /// Set to <value>true</value> if this method is called by user.
    /// Otherwise set to <value>false</value> when called by GC during finalization stage.
    /// </param>
    protected virtual void OnDisposing(Boolean disposedByUser) {

      // IMPORTANT: Dispose managed resources only if 'disposedByUser' set to 'true'.
      //            Managed resources are not available during the finalization stage!
      if (disposedByUser) {
        // Dispose managed resources

        // this.connection?.Dispose();
        // this.otherClass?.Dispose();
      }

      // Clean up unmanaged resources

      // if (handle != IntPtr.Zero) CloseHandle(handle);
      // handle = IntPtr.Zero;

      // IMPORTANT: Do not forget to call base class implementation!
      // base.OnDisposing(disposedByUser);
    }

    #endregion IDisposable implementation

    #region IsDisposed

    /// <summary>
    /// Indicates if object is already disposed or not.
    /// For nested classes internal use only.
    /// </summary>
    protected Boolean IsDisposed { get; private set; } = false;

    #endregion IsDisposed

  }

  #endregion [Disposable type definition]

}
