namespace FileDownload.Data {

  #region [JobStatus enum definition]

  /// <summary>
  /// Defines supported job (processing) statuses.
  /// </summary>
  public enum JobStatus {

    /// <summary>
    /// (Default) Job has been scheduled (created).
    /// </summary>
    Created = 0,

    /// <summary>
    /// Job is in progress.
    /// </summary>
    Processing,

    /// <summary>
    /// Job has been complete.
    /// </summary>
    Complete

  }

  #endregion [JobStatus enum definition]

}
