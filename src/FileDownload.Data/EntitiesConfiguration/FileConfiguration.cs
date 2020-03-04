using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileDownload.Data.EntitiesConfiguration {

  #region [FileConfiguration type definition]

  /// <summary>
  /// This class defines additional metadata configuration for <seealso cref="File"/> entity.
  /// </summary>
  internal sealed class FileConfiguration : IEntityTypeConfiguration<File> {

    #region IEntityStatusConfiguration implementation

    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<File> builder) {
      builder.ToTable($"{nameof(File)}s");
      builder.HasKey(e => new { e.JobId, e.Name });

      builder.Property(e => e.JobId)
        .HasConversion<Guid>(val => val.Guid, val => new Types.ShortGuid(val)).IsUnicode(false);
    }

    #endregion IEntityStatusConfiguration implementation

  }

  #endregion [FileConfiguration type definition]

}
