
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FileDownload.Api.Models;
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

    #region [Get] [controller]/{id}

    /// <summary>
    /// Reports <seealso cref="Job"/> status and associated <seealso cref="File"/>(s) details.
    /// </summary>
    /// <param name="id">Unique <seealso cref="Job"/> identifier.</param>
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

      var qry = db.Set<Job>().AsQueryable()
        .Include(e => e.Files)
        .Where(j => j.Id == jobId);
      var res = await qry.SingleOrDefaultAsync();

      //
      // Report result
      //

      if (res == null) return NotFound();
      return Ok(res);
    }

    #endregion [Get] [controller]

    #region [Post] [controller]

    /// <summary>
    /// Creates new <seealso cref="Job"/>.
    /// </summary>
    /// <param name="files">List of <seealso cref="File"/>(s) to download.</param>
    /// <returns>Newly created <seealso cref="Job"/> details.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Job>> CreateJob(CreateJobModel data) {

      //
      // Validate input
      //

      if (data == null) return BadRequest();

      //
      // Perform action
      //

      var db = this._context;

      var jobId = ShortGuid.NewGuid();
      var job = new Job(jobId) {
        Status = JobStatus.Created,
        Threads = data.Threads,
        Files = data.Links.Select(f => new File(jobId, f.Name) { Url = f.Url }).ToArray()
      };
      db.Add(job);

      try {
        await db.SaveChangesAsync();
      } catch (DbUpdateException ex) {
        var dbEx = ex.InnerException as System.Data.Common.DbException;
        if (dbEx != null)
          return ValidationProblem(new ValidationProblemDetails(new Dictionary<String, String[]>() {
            { $"{dbEx.ErrorCode}", new [] { dbEx.Message } }
          }));

        throw;
      }

      //
      // Report result
      //

      return CreatedAtAction("Get", new { id = job.Id }, job);
    }

    #endregion [Post] [controller]

  }

  #endregion [JobsController type definition]

}
