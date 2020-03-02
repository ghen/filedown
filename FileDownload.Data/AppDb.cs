using System.Reflection;

using Microsoft.EntityFrameworkCore;

namespace FileDownload.Data {

  #region [AppDb class definiiton]

  /// <summary>
  /// Provides access to application database entities and services.
  /// </summary>
  public sealed class AppDb : DbContext {

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="AppDb"/> instance.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
      optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
      base.OnConfiguring(optionsBuilder);
    }

    #endregion Constructor and Initialization

    #region OnModelCreating

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      var assembly = Assembly.GetAssembly(typeof(AppDb));
      modelBuilder.ApplyConfigurationsFromAssembly(
        assembly,
        (type) => type.Namespace?.Contains("EntitiesConfiguration") ?? false
      );
    }

    #endregion OnModelCreating

  }

  #endregion [AppDb class definiiton]

}
