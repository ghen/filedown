
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FileDownload.Data;
using FileDownload.Types;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FileDownload.Api.Controllers {

  #region [JobsController type definition]

  /// <summary>
  /// Provides access to <see cref="Job"/>(s) management tasks.
  /// </summary>
  public class JobsController : ApiControllerBase {

    #region Private members

    private readonly DbContext _context;

    #endregion Private members

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="JobsController"/> instance.
    /// </summary>
    /// <param name="context">Data access context.</param>
    /// <param name="logger">(Optional) Logger to send disagnostic messages to.</param>
    public JobsController(DbContext context, ILogger<JobsController> logger = null) : base(logger) {
      this._context = context ?? throw new ArgumentNullException(nameof(context));
    }

    #endregion Constructor and Initialization

    #region [Get] api/[controller]/{id}

    /// <summary>
    /// Reports <seealso cref="Job"/> status and associated <seealso cref="File"/>(s) details.
    /// </summary>
    /// 
    /// <returns>List of matching records or empty set.</returns>
    [HttpGet, Route("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<File>>> GetJob(String id) {

      //
      // Validate input
      //

      var jobId = new ShortGuid(id);
      if (jobId == ShortGuid.Empty) return BadRequest();

      //
      // Perform action
      //

      var db = this._context;

      // DEBUG: temporary code to inject new records into the storage
      var file = new File(jobId, $"{jobId}.tmp") {
        Url = String.Empty
      };
      await db.AddAsync(file);
      await db.SaveChangesAsync();
      // END DEBUG

      var qry = db.Set<File>().AsQueryable();
      var res = await qry.ToListAsync();

      //
      // Report result
      //

      return Ok(res);
    }

    #endregion [Get] api/[controller]

  }

  #endregion [JobsController type definition]

}
