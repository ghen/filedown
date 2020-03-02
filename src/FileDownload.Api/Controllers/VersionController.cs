using System;
using System.Reflection;

using FileDownload.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FileDownload.Api.Controllers {

  #region [VersionController type definition]

  /// <summary>
  /// Reports API version information.
  /// </summary>
  public class VersionController : ApiControllerBase {

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="VersionController"/> instance.
    /// </summary>
    /// <param name="logger">(Optional) Logger to send disagnostic messages to.</param>
    public VersionController(ILogger<VersionController> logger = null) : base(logger) { }

    #endregion Constructor and Initialization

    #region [Get] api/[controller]

    /// <summary>
    /// Lists all records that match search filters.
    /// </summary>
    /// <param name="skip">(Optional) Skip first N records.</param>
    /// <param name="take">(Optional) Return top N records.</param>
    /// <returns>List of matching records or empty set.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<String> ApiVersion() {

      var assembly = Assembly.GetAssembly(typeof(VersionController));
      var res = assembly.VersionString();

      return Ok(res);
    }

    #endregion [Get] api/[controller]

  }

  #endregion [VersionController type definition]

}
