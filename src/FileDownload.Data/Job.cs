using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using FileDownload.Types;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FileDownload.Data {

  #region [Job type definition]

  /// <summary>
  /// Stores <see cref="Job"/> details.
  /// </summary>
  [DataContract]
  public sealed class Job {

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="File"/> instance.
    /// </summary>
    /// <param name="id">Uniqueue <see cref="Job"/> identifier.</param>
    public Job(ShortGuid id) {
      this.Id = id;
    }

    #endregion Constructor and Initialization

    #region Artificial Properties

    /// <summary>
    /// Proxy property to support <seealso cref="DataContractAttribute"/> serrialization.
    /// </summary>
    [DataMember(Name = "id"), DataType(DataType.Text)]
    private String IdStr {
      get => this.Id.Value;
      set => this.Id = new ShortGuid(value);
    }

    #endregion Artificial Properties

    #region Properties

    /// <summary>
    /// Uniqueue <see cref="Job"/> identifier.
    /// </summary>
    [Required]
    public ShortGuid Id { get; private set; }

    /// <summary>
    /// Status.
    /// </summary>
    [DataMember(Name = "status")]
    [JsonConverter(typeof(StringEnumConverter))]
    [Required, MaxLength(20)]
    public JobStatus Status { get; set; }

    /// <summary>
    /// Level of parallelism. If set to <value>0</value> (zero), default system settings would apply.
    /// </summary>
    [DataMember(Name = "threads")]
    [Required, Range(0, 24)]
    public Int32 Threads { get; set; }

    #endregion Properties

    #region Navigation properties

    /// <summary>
    /// List of <seealso cref="File"/>(s) associated with this <see cref="Job"/>.
    /// </summary>
    [DataMember(Name = "files")]
    public ICollection<File> Files { get; set; }

    #endregion Navigation properties

    #region ToString

    /// <inheritdoc/>
    public override String ToString()
      => $"'{this.Id}' ({this.Status}) [{this.Files.Count} file(s)]";

    #endregion ToString

  }

  #endregion [Job type definition]

}
