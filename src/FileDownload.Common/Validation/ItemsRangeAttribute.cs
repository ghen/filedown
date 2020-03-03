using System;
using System.ComponentModel.DataAnnotations;

namespace FileDownload.Validation {

  #region [ItemsRangeAttribute type definition]

  /// <summary>
  /// Validates items count in collections.
  /// </summary>
  public sealed class ItemsRangeAttribute : ValidationAttribute {

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="ItemsRangeAttribute"/> instance.
    /// </summary>
    /// <param name="minCount">Minilal items count.</param>
    /// <param name="maxCount">Maximum items count.</param>
    public ItemsRangeAttribute(UInt32 minCount = 0, UInt32 maxCount = Int32.MaxValue) {
      this.MinCount = minCount;
      this.MaxCount = maxCount;
    }

    #endregion Constructor and Initialization

    #region Properties

    /// <summary>
    /// Minilal items count.
    /// </summary>
    public UInt32 MinCount { get; private set; }

    /// <summary>
    /// Maximum items count.
    /// </summary>
    public UInt32 MaxCount { get; private set; }

    #endregion Properties

    #region ValidationAttribute overrides

    /// <inheritdoc/>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext) {

      var items = value as System.Collections.ICollection;
      if (items == null)
        return base.IsValid(value, validationContext);

      if (items.Count < this.MinCount)
        return new ValidationResult($"Collection shold have at least {this.MinCount} element(s).");

      if (items.Count > this.MaxCount)
        return new ValidationResult($"Collection shold have no more than {this.MinCount} element(s).");

      return ValidationResult.Success;
    }

    #endregion ValidationAttribute overrides

  }

  #endregion [ItemsRangeAttribute type definition]

}
