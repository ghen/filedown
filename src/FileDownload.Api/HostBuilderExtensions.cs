using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FileDownload.Api {

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

    #region ConfigureApiHost

    /// <summary>
    /// Configures host to run REST API Server.
    /// </summary>
    /// <param name="hostBuilder">Host builder to configure.</param>
    /// <exception cref="ArgumentNullException">Throws if <paramref name="hostBuilder"/> is not set (<value>null</value>).</exception>
    /// <returns>Source <seealso cref="IWebHostBuilder"/> to support methods chaining.</returns>
    public static IWebHostBuilder ConfigureApiHost(this IWebHostBuilder hostBuilder)
      => throw new NotImplementedException();

    /// <summary>
    /// Configures host to run REST API Server.
    /// </summary>
    /// <param name="hostBuilder">Host builder to configure.</param>
    /// <exception cref="ArgumentNullException">Throws if <paramref name="hostBuilder"/> is not set (<value>null</value>).</exception>
    /// <returns>Source <seealso cref="IWebHostBuilder"/> to support methods chaining.</returns>
    public static IHostBuilder ConfigureApiHost(this IHostBuilder hostBuilder) {
      if (hostBuilder == null) throw new ArgumentNullException(nameof(hostBuilder));

      hostBuilder
        .ConfigureAppConfiguration((hostContext, config) => config
          .AddJsonFile("api.json", optional: false)
          .AddJsonFile($"api.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true))
        .ConfigureWebHostDefaults(webBuilder => {
          webBuilder.UseStartup<Startup>();
        });

      return hostBuilder;
    }

    #endregion ConfigureApiHost

  }

  #endregion [HostBuilderExtensions type definition]

}
