using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FileDownload.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileDownload.Services {

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

    #region ConfigureServicesHost

    /// <summary>
    /// Configures host to run REST API Server.
    /// </summary>
    /// <param name="hostBuilder">Host builder to configure.</param>
    /// <exception cref="ArgumentNullException">Throws if <paramref name="hostBuilder"/> is not set (<value>null</value>).</exception>
    /// <returns>Source <seealso cref="IWebHostBuilder"/> to support methods chaining.</returns>
    public static IHostBuilder ConfigureServicesHost(this IHostBuilder hostBuilder) {
      if (hostBuilder == null) throw new ArgumentNullException(nameof(hostBuilder));

      hostBuilder
        .ConfigureAppConfiguration((hostContext, config) => config
          .AddJsonFile("services.json", optional: false)
          .AddJsonFile($"services.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true))
        .ConfigureServices((_, services) => {

          // NOTE: For the purpose of this sample application we would use the simplest available
          //       producer-consumer queue implementation.
          services.AddSingleton(new BlockingCollection<Job>());

          services.AddSingleton(ctx =>
            ctx
              .GetRequiredService<IConfiguration>()
              .GetSection(nameof(FileDownloadService).Replace("Service", String.Empty))
              .Get<FileDownloadServiceSettings>(config => config.BindNonPublicProperties = true)
              ?? new FileDownloadServiceSettings()  
          );
          services.AddScoped<FileDownloadService>();
          services.AddHostedService<HostedService<FileDownloadService>>();

        });

      return hostBuilder;
    }

    #endregion ConfigureServicesHost

  }

  #endregion [HostBuilderExtensions type definition]

}
