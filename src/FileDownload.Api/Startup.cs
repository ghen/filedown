using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileDownload.Api {

  #region [Startup type definition]

  /// <summary>
  /// Application startup configuration.
  /// </summary>
  public class Startup {

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="Startup"/> instance.
    /// </summary>
    /// <param name="configuration">Application configuration details.</param>
    public Startup(IConfiguration configuration) {
      this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    #endregion Constructor and Initialization

    #region Properties

    /// <summary>
    /// Application configuration details.
    /// </summary>
    private IConfiguration Configuration { get; }

    #endregion Properties

    #region ConfigureServices

    /// <summary>
    /// This method gets called by the runtime. Use this method to add services to the container.
    /// </summary>
    /// <remarks>
    /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
    /// </remarks>
    /// <param name="services">Services owner configurator.</param>
    public void ConfigureServices(IServiceCollection services) =>
      services
        .AddRouting()
        .AddControllers()
        .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);

    #endregion ConfigureServices

    #region Configure

    /// <summary>
    /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Application builder instance reference.</param>
    /// <param name="env">Hosting environment details.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      } else {
        app.UseHsts();
      }

      // NOTE: Order is important
      app
        .UseRouting()
        .UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
    }

    #endregion Configure

  }

  #endregion [Startup type definition]

}