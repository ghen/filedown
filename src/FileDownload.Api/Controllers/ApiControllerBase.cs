using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace FileDownload.Api.Controllers {

  #region [ApiControllerBase type definition]

  /// <summary>
  /// Implements basic functionality for all WEB.API controllers.
  /// </summary>
  [ApiController, Route("api/[controller]")]
  [Produces("application/json")]
  public class ApiControllerBase : ControllerBase {

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="ApiControllerBase"/> instance.
    /// </summary>
    /// <param name="logger">(Optional) Logger to send disagnostic messages to.</param>
    public ApiControllerBase(ILogger<ApiControllerBase> logger = null) {
      this.Log = logger ?? NullLogger<ApiControllerBase>.Instance;
    }

    #endregion Constructor and Initialization

    #region Properties

    /// <summary>
    /// Logger to send disagnostic messages to.
    /// </summary>
    protected ILogger<ApiControllerBase> Log { get; }

    #endregion Properties

  }

  #endregion [ApiControllerBase type definition]

  #region [ApiControllerBase<TConf> type definition]

  /// <summary>
  /// Implements basic functionality for all WEB.API controllers.
  /// </summary>
  /// <typeparam name="TConf">Controller configuration type.</typeparam>
  public class ApiControllerBase<TConf> : ApiControllerBase
    where TConf : class, new() {

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="ApiControllerBase"/> instance.
    /// </summary>
    /// <param name="conf">Controller configuration.</param>
    /// <param name="logger">(Optional) Logger to send disagnostic messages to.</param>
    public ApiControllerBase(IOptions<TConf> conf, ILogger<ApiControllerBase> logger = null) : base(logger) {
      this.Config = conf.Value ?? throw new ArgumentNullException(nameof(conf));
    }

    #endregion Constructor and Initialization

    #region Properties

    /// <summary>
    /// Controller configuration.
    /// </summary>
    protected TConf Config { get; }

    #endregion Properties

  }

  #endregion [ApiControllerBase<TConf> type definition]

}
