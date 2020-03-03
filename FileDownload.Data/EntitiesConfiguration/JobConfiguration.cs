using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileDownload.Data.EntitiesConfiguration {

  #region [JobConfiguration type definition]

  /// <summary>
  /// This class defines additional metadata configuration for <seealso cref="Job"/> entity.
  /// </summary>
  internal sealed class JobConfiguration : IEntityTypeConfiguration<Job> {

    #region IEntityStatusConfiguration implementation

    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Job> builder) {
      builder.ToTable($"{nameof(Job)}s");
      builder.HasKey(e => e.Id);

      builder.Property(e => e.Id)
        .HasConversion<Guid>(val => val.Guid, val => new Types.ShortGuid(val)).IsUnicode(false);
      builder.Property(e => e.Status)
        .HasConversion<String>().IsUnicode(false);

      builder
        .HasMany(e => e.Files)
        .WithOne()
        .HasForeignKey(e => e.JobId);
    }

    #endregion IEntityStatusConfiguration implementation

  }

  #endregion [JobConfiguration type definition]

}
