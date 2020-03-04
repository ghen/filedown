using System;
using System.Runtime.Serialization;

namespace FileDownload.Services {

  #region [AppServiceSettings type definition]

  /// <summary>
  /// Basic <seealso cref="AppService"/> service settings. 
  /// </summary>
  [DataContract]
  public class AppServiceSettings {

    #region Properties

    /// <summary>
    /// (Optional) Display name.
    /// </summary>
    [DataMember(Name = "name")]
    public String Name { get; private set; }

    #endregion Properties

  }

  #endregion [AppServiceSettings type definition]

}
