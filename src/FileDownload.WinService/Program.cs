using System;
using System.Linq;
using System.Reflection;

using FileDownload.Api;
using FileDownload.Data;
using FileDownload.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NLog.Extensions.Logging;

namespace FileDownload.WinService {

  #region [Program type definition]

  /// <summary>
  /// Runs SHIM within a Windows Service instance.
  /// </summary>
  internal static class Program {

    #region Private members

    private static NLog.ILogger Log = NLog.LogManager.GetLogger(typeof(Program).FullName);

    #endregion Private members

    #region Main

    /// <summary>
    /// Application enrty point.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    private static Int32 Main(String[] args) {
      try {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
        Log.Warn($"{assembly.VersionString()} (build: {assembly.BuildString()})");

        // NOTE: .NET Core 3.x CommandLineConfigurationProvider does not support "parameters with no value"
        //       See also: https://github.com/aspnet/Configuration/issues/780
        var isConsoleMode = (Array.IndexOf(args, "--console") >= 0);
        if (isConsoleMode) args = args.Where(a => a != "--console").ToArray();

        //
        // Set-up application Host
        //
        var builder = new HostBuilder()
          .ConfigureHostConfiguration((config) => config
            .AddEnvironmentVariables("NETCORE_").AddCommandLine(args));

        //
        // Configure application components
        //
        builder
          .ConfigureAppConfiguration((_, config) => config
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory))
          .ConfigureAppConfiguration((hostContext, config) => config
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true))
          .ConfigureLogging((hostContext, logging) => logging
            .AddConfiguration(hostContext.Configuration.GetSection("Logging"))
            .ClearProviders().AddNLog())
          .ConfigureServices((hostContext, services) => services
            .AddOptions()
            .Configure<HostOptions>(hostContext.Configuration.GetSection("Host")))
          .ConfigureDataProviders()
          .ConfigureApiHost()
          // .ConfigureServicesHost()
          .ConfigureAppConfiguration((_, config) => config
            .AddCommandLine(args));

        //
        // Configure Service vs Console
        //
        if (!isConsoleMode) {
          // IMPORTANT: Update working directory when running as a Windows Service
          System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);

          builder
            .ConfigureServices((_, services) => services.AddSingleton<IHostLifetime, Service>());
        } else {
          builder
            // HACK: Supress startup messages written by HostBuilder directly into Console!
            .ConfigureServices((hostContext, services) => services
              // TODO: Find where is this SH*T is read from config by default!
              //       See also: https://github.com/aspnet/Extensions/issues/1103
              .Configure<ConsoleLifetimeOptions>(hostContext.Configuration.GetSection("Console")))
            .UseConsoleLifetime();
        }

        //
        // Execute
        //
        builder.Build().Run();

        Log.Warn("Done.");
        Log.Info(String.Empty);
        Log.Info(String.Empty);
        Log.Info(String.Empty);

      } catch (Exception ex) {

        try {
          Log.Error(ex, ex.Message);
        } catch (Exception) {
          //
          // NOTE: Keep console output in place in case someone screw logs configuration
          //
          Console.ForegroundColor = ConsoleColor.Red;
          Console.Error.WriteLine();
          Console.Error.WriteLine($"FATAL: {ex.DetailedMessage()}");
          Console.Error.WriteLine();
          Console.ResetColor();
        }

        return -1;
      } finally {
        NLog.LogManager.Shutdown();
      }

      return 0;
    }

    #endregion Main

  }

  #endregion [Program type definition]

}
