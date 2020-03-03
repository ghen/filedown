using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

using FileDownload.Data;
using FileDownload.Validation;

namespace FileDownload.Api.Models {

  #region [CreateJobModel type definition]

  /// <summary>
  /// Stores new <seealso cref="Job"/> requirements.
  /// </summary>
  [DataContract]
  public sealed class CreateJobModel : IValidatableObject {

    #region Properties

    /// <summary>
    /// Level of parallelism.
    /// </summary>
    [DataMember(Name = "threads")]
    [Required, Range(0, 24)]
    public Int32 Threads { get; set; }

    /// <summary>
    /// List of files to download.
    /// </summary>
    [DataMember(Name = "links")]
    [Required, ItemsRange(minCount: 1)]
    public ICollection<File> Links { get; set; }

    #endregion Properties

    #region IValidatableObject support

    /// <inheritdoc/>
    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext) {

      //
      // Detect duplicate file names
      // to meet the requirement early, and to prevent EF validation errors later on.
      //

      var files = this.Links;
      if (files != null) {
        var duplicates = files
          .GroupBy(f => f.Name)
          .Where(g => g.Count() > 1);

        foreach (var g in duplicates)
          yield return new ValidationResult(
              $"File name should be unique. Duplicate file name in use: '{g.Key}'.",
              new[] { nameof(CreateJobModel.Links) });
      }

    }

    #endregion IValidatableObject support

  }

  #endregion [CreateJobModel type definition]

}
