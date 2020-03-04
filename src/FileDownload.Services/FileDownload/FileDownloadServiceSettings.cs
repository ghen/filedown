using System;
using System.Runtime.Serialization;

namespace FileDownload.Services {

  #region [FileDownloadServiceSettings type definition]

  /// <summary>
  /// <seealso cref="FileDownloadService"/> service settings.
  /// </summary>
  [DataContract]
  public sealed class FileDownloadServiceSettings : AppServiceSettings {

    /// <summary>
    /// (Optional) Display name.
    /// </summary>
    //[DataMember(Name = "name")]
    //public String Name { get; private set; }

  }

  #endregion [FileDownloadServiceSettings type definition]

}
