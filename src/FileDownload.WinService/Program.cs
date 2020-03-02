using System;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NLog.Extensions.Logging;

using FileDownload.Extensions;

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
        Log.Warn($"{Program.AppVersionStr()} (build: {Program.BuildVersionStr()})");

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

    #region Internal helpers

    /// <summary>
    /// Reports application name and version.
    /// </summary>
    /// <returns>Application name and version.</returns>
    public static String AppVersionStr() {
      var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
      var version = assembly.GetName().Version.ToString(3);
      var title = ((AssemblyTitleAttribute)assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true)[0])
          .Title;

      var res = $"{title} v{version}";
      return res;
    }

    /// <summary>
    /// Reports full build version.
    /// </summary>
    /// <returns>Full build version.</returns>
    public static String BuildVersionStr() {
      var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
      var version = ((AssemblyInformationalVersionAttribute)assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), true)
          .FirstOrDefault())?.InformationalVersion ?? assembly.GetName().Version.ToString();

      var conf = (String)null;
#if DEBUG
      conf = ((AssemblyConfigurationAttribute)assembly.GetCustomAttributes(
          typeof(AssemblyConfigurationAttribute), true)[0]).Configuration;
#endif

      var res = $"v{version}{(!String.IsNullOrEmpty(conf) ? $" ({conf})" : String.Empty)}";
      return res;
    }

    #endregion Internal helpers

  }

  #endregion [Program type definition]

}
