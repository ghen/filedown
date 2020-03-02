using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using FileDownload.Types;

namespace FileDownload.Data {

  #region [File type definition]

  /// <summary>
  /// Stores File details.
  /// </summary>
  [DataContract]
  public sealed class File {

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="File"/> instance.
    /// </summary>
    /// <param name="jobId">Uniqueue <see cref="Job"/> identifier.</param>
    /// <param name="name">Unique file name (in scope of current <see cref="Job"/>).</param>
    public File(ShortGuid jobId, String name) {
      this.JobId = jobId;
      this.Name = name;
    }

    #endregion Constructor and Initialization

    #region Properties

    /// <summary>
    /// Uniqueue <see cref="Job"/> identifier.
    /// </summary>
    [DataMember(Name = "job")]
    [Required]
    public ShortGuid JobId { get; private set; }

    /// <summary>
    /// Unique file name (in scope of current <see cref="Job"/>).
    /// </summary>
    [DataMember(Name = "filename")]
    [Required, MaxLength(128)]
    public String Name { get; private set; }

    /// <summary>
    /// Url.
    /// </summary>
    [DataMember(Name = "link")]
    [Required, StringLength(1024)]
    public String Url { get; set; }

    /// <summary>
    /// Date and Time when file download started.
    /// </summary>
    [DataMember(Name = "started")]
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// Date and Time when file download finished.
    /// </summary>
    [DataMember(Name = "finished")]
    public DateTimeOffset? FinishedAt { get; set; }

    /// <summary>
    /// Total bytes downloaded (if successful).
    /// </summary>
    [DataMember(Name = "bytes")]
    public Int32? Bytes { get; set; }

    /// <summary>
    /// Error message (if download failed).
    /// </summary>
    [DataMember(Name = "error")]
    [MaxLength(1024)]
    public String Error { get; set; }

    #endregion Properties

    #region Navigation properties

    /// <summary>
    /// Job this entity belongs to.
    /// </summary>
    // public Job Job { get; set; }

    #endregion Navigation properties

    #region ToString

    /// <inheritdoc/>
    public override String ToString()
      => $"'{this.Name}' ({this.Bytes/1024:F0}KB) [{this.JobId}]";

    #endregion ToString

  }

  #endregion [File type definition]

}
