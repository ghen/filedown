using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;

using FileDownload.Data;
using FileDownload.Extensions;
using FileDownload.Types;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FileDownload.Services {

  #region [FileDownloadService type definition]

  /// <summary>
  /// Processes scheduled <seealso cref="Data.Job"/>(s) asynchronously.
  /// </summary>
  public sealed class FileDownloadService : AppService<FileDownloadServiceSettings> {

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="FileDownloadService"/> instance.
    /// </summary>
    /// <param name="serviceProvider">Service provider.</param>
    /// <param name="settings">Service execution settings.</param>
    /// <param name="logger">(Optional) Logger to send disagnostic messages to.</param>
    public FileDownloadService(IServiceProvider serviceProvider, FileDownloadServiceSettings settings, ILogger<FileDownloadService> logger = null)
      : base(serviceProvider, settings, logger) { }

    #endregion Constructor and Initialization

    #region AppService overrides

    /// <inheritdoc/>
    public override void Execute(CancellationToken cancellationToken = default) {
      var conf = this.Config;
      var services = this.ServiceProvider;

      var queue = LookupJobsQueue(services);
      var jobs = queue.GetConsumingEnumerable(cancellationToken);

      jobs.AsParallel()
        .WithDegreeOfParallelism(conf.Jobs)
        .WithCancellation(cancellationToken)
        .ForAll(job => this.ProcessJob(job, cancellationToken: cancellationToken));

      //
      // LookupJobsQueue
      //
      BlockingCollection<Job> LookupJobsQueue(IServiceProvider services)
        => (services.GetService(typeof(BlockingCollection<Job>)) as BlockingCollection<Job>)
          ?? throw new ApplicationException("Jobs processing queue is not available.");
    }

    #endregion AppService overrides

    #region ProcessJob

    /// <summary>
    /// Download all files associated with the <paramref name="job"/> and marks <paramref name="job"/> as <seealso cref="JobStatus.Complete"/>.
    /// </summary>
    /// <param name="job"><seealso cref="Job"/> to process.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    private void ProcessJob(Job job, CancellationToken cancellationToken = default(CancellationToken)) {
      if (job == null) throw new ArgumentNullException(nameof(job));

      var conf = this.Config;
      var services = this.ServiceProvider;

      try {
        var watch = Stopwatch.StartNew();
        var threads = (job.Threads != 0) ? job.Threads : conf.Threads;

        Log.Debug("Processing job {job} [{threads} thread(s)]...", job, threads);
        job.Status = JobStatus.Processing;
        UpdateJobDetails(job);

        job.Files.AsParallel()
          .WithDegreeOfParallelism(threads)
          .WithCancellation(cancellationToken)
          .ForAll(file => this.DownloadFile(file, cancellationToken: cancellationToken));

        job.Status = JobStatus.Complete;
        UpdateJobDetails(job);
        Log.Debug("Job {job} complete [{elapsed:hh\\:mm\\:ss\\.f}].", job, watch.Elapsed);

      } catch (Exception ex) when (ex.GetType() != typeof(OperationCanceledException)) {
        Log.Error(ex, "{job} failed with error.", job);
      }

      //
      // LookupDbContext
      //
      DbContext LookupDbContext(IServiceProvider services)
        => services.GetService(typeof(DbContext)) as DbContext
          ?? throw new ApplicationException("Data storage is not available.");

      //
      // UpdateJobDetails
      //
      void UpdateJobDetails(Job job) {
        var db = LookupDbContext(services);
        db.Attach(job).State = EntityState.Modified;
        db.SaveChanges();
      }
    }

    #endregion ProcessJob

    #region DownloadFile

    /// <summary>
    /// Downloads <paramref name="file"/> and update its stats.
    /// </summary>
    /// <param name="file"><seealso cref="File"/> to download.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    private void DownloadFile(File file, CancellationToken cancellationToken = default(CancellationToken)) {
      if (file == null) throw new ArgumentNullException(nameof(file));

      var conf = this.Config;
      var services = this.ServiceProvider;

      try {
        var watch = Stopwatch.StartNew();

        Log.Debug("Downloading {file}...", file);
        file.StartedAt = DateTimeOffset.Now;
        file.FinishedAt = null;
        file.Size = 0;
        file.Bytes = null;
        file.Error = null;
        UpdateFileDetails(file);

        using (var client = new HttpClient()) {
          var resp = client.GetAsync(file.Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken: cancellationToken)
            .GetAwaiter().GetResult();

          resp.EnsureSuccessStatusCode();
          if (resp.Content != null) {
            file.Size = (Int32)(resp.Content.Headers.ContentLength ?? 0);     // TODO: Ops... Possible overflow!!
            file.Bytes = 0;
            UpdateFileDetails(file);

            var path = System.IO.Path.Combine(conf.Downloads, file.JobId, file.Name);
            var fileInfo = new System.IO.FileInfo(path);
            if (!fileInfo.Directory.Exists) fileInfo.Directory.Create();

            const Int32 TRANSFER_BUFFER_SIZE = 8 * 1024;
            using (var dest = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None, TRANSFER_BUFFER_SIZE, true))
            // TODO: Double-check that ReadAsStreamAsync(..) does not read the whole stream into local memory buffer!
            using (var src = resp.Content.ReadAsStreamAsync().GetAwaiter().GetResult()) {
              // NOTE: CopyToAsync(..) does not provide any progress info
              // src.CopyToAsync(dest, cancellationToken: cancellationToken);

              var len = 0;
              var buf = new Byte[TRANSFER_BUFFER_SIZE];
              while ((len = src.Read(buf, 0, buf.Length)) > 0) {
                dest.Write(buf, 0, len);

                // NOTE: Accessing Data Storage every iteration would be very expensive
                // NOTE: DO NOT USE THIS CODE IN PRODUCTION!!
                file.Bytes += len;
                UpdateFileDetails(file);
                // END NOTE
              }
            }
          }
        }

        file.FinishedAt = DateTimeOffset.Now;
        UpdateFileDetails(file);
        Log.Debug("{file} download complete [{elapsed:hh\\:mm\\:ss\\.f}].", file, watch.Elapsed);

      } catch (Exception ex) when (ex.GetType() != typeof(OperationCanceledException)) {
        Log.Error(ex, "{file} download failed with error.", file);

        try {
          file.Error = ex.DetailedMessage();
          file.FinishedAt = DateTimeOffset.Now;
          UpdateFileDetails(file);
        } catch (Exception ex2) {
          Log.Debug(ex2, "Unable to persist {file} changes.", file);
        }
      }

      //
      // LookupDbContext
      //
      DbContext LookupDbContext(IServiceProvider services)
        => services.GetService(typeof(DbContext)) as DbContext
          ?? throw new ApplicationException("Data storage is not available.");

      //
      // UpdateFileDetails
      //
      void UpdateFileDetails(File file) {
        var db = LookupDbContext(services);
        db.Attach(file).State = EntityState.Modified;
        db.SaveChanges();
      }
    }

    #endregion DownloadFile

  }

  #endregion [FileDownloadService type definition]

}
