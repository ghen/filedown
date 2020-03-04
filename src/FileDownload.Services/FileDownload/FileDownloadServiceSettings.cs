using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace FileDownload.Services {

  #region [FileDownloadServiceSettings type definition]

  /// <summary>
  /// <seealso cref="FileDownloadService"/> service settings.
  /// </summary>
  [DataContract]
  public sealed class FileDownloadServiceSettings : AppServiceSettings {

    /// <summary>
    /// Number of maximum allowed parallel jobs.
    /// </summary>
    [DataMember(Name = "jobs")]
    [Range(0, 24)]
    public Int32 Jobs { get; private set; } = 3;

    /// <summary>
    /// Default number of threads per job.
    /// </summary>
    [DataMember(Name = "threads")]
    [Range(0, 24)]
    public Int32 Threads { get; private set; } = 5;

    /// <summary>
    /// Path to the local folder to store downloaded files.
    /// </summary>
    [DataMember(Name = "downloads")]
    [Required, MaxLength(1024)]
    public String Downloads { get; private set; }

  }

  #endregion [FileDownloadServiceSettings type definition]

}
