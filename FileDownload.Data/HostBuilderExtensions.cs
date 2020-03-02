using System;
using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileDownload.Data {

  #region [HostBuilderExtensions type definition]

  /// <summary>
  /// Provides initialization logic that can be applied to <seealso cref="IWebHostBuilder"/> to prepare host for REST API execution.
  /// </summary>
  /// <remarks>
  /// The idea of this class is to decouple platform- and hosting-dependent initialization logic
  /// (<seealso cref="FileDownload.WinService.Program"/> as an example) and (ultimately) cross-platform
  /// application .NET Core code.
  /// </remarks>
  public static class HostBuilderExtensions {

    #region ConfigureDataProviders

    /// <summary>
    /// Configures host to run SHIM Server.
    /// </summary>
    /// <param name="hostBuilder">Host builder to configure.</param>
    /// <exception cref="ArgumentNullException">Throws if <paramref name="hostBuilder"/> is not set (<value>null</value>).</exception>
    /// <returns>Source <seealso cref="IHostBuilder"/> to support methods chaining.</returns>
    public static IHostBuilder ConfigureDataProviders(this IHostBuilder hostBuilder) {
      if (hostBuilder == null) throw new ArgumentNullException(nameof(hostBuilder));

      hostBuilder
        .ConfigureServices((hostContext, services) => {
          var assembly = Assembly.GetAssembly(typeof(HostBuilderExtensions));
          var options = new DbContextOptionsBuilder<AppDb>()
            // NOTE: For the purpose of this demo application we are going to use in-memory database
            // .UseSqlServer(this.Configuration.GetConnectionString(nameof(AppDb))
            .UseInMemoryDatabase(databaseName: assembly.GetName().Name)
            // NOTE: Forbid client-side queries evaluations
            //       https://docs.microsoft.com/en-au/ef/core/querying/client-eval
            // .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning))
            .Options;

          services
            .AddScoped<DbContext>(serviceProvider => {
              return new AppDb(options);
            });
        });

      return hostBuilder;
    }

    #endregion ConfigureApiHost

  }

  #endregion [HostBuilderExtensions type definition]

}
