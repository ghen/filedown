using System;
using System.Linq;

namespace FileDownload.Types {

  #region [ShortGuid class definition]

  /// <summary>
  /// Represents a globally unique identifier (GUID) with a shorter String value (Base-64 encoded).
  /// </summary>
  public struct ShortGuid {

    #region Static

    /// <summary>
    /// A read-only instance of the ShortGuid class whose value
    /// is guaranteed to be all zeroes.
    /// </summary>
    public static readonly ShortGuid Empty = new ShortGuid(Guid.Empty);

    #endregion

    #region Contructors

    /// <summary>
    /// Creates a ShortGuid from a base64 encoded String
    /// </summary>
    /// <param name="value">The encoded guid as a
    /// base64 String</param>
    public ShortGuid(String value) {
      this.Value = value;
      this.Guid = ShortGuid.Decode(value);
    }

    /// <summary>
    /// Creates a ShortGuid from a Guid
    /// </summary>
    /// <param name="guid">The Guid to encode</param>
    public ShortGuid(Guid guid) {
      this.Guid = guid;
      this.Value = ShortGuid.Encode(guid);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets/sets the underlying Guid
    /// </summary>
    public Guid Guid { get; }

    /// <summary>
    /// Gets/sets the underlying base64 encoded String
    /// </summary>
    public String Value { get; }

    #endregion

    #region ToString

    /// <summary>
    /// Returns the base64 encoded guid as a String
    /// </summary>
    /// <returns></returns>
    public override String ToString()
        => this.Value;

    #endregion

    #region Equals

    /// <summary>
    /// Returns a value indicating whether this instance and a
    /// specified Object represent the same type and value.
    /// </summary>
    /// <param name="obj">The object to compare</param>
    /// <returns></returns>
    public override bool Equals(object obj) {
      if (obj is ShortGuid)
        return this.Guid.Equals(((ShortGuid)obj).Guid);
      if (obj is Guid)
        return this.Guid.Equals((Guid)obj);
      if (obj is String)
        return this.Guid.Equals(((ShortGuid)obj).Guid);
      return false;
    }

    #endregion

    #region GetHashCode

    /// <summary>
    /// Returns the HashCode for underlying Guid.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
        => this.Guid.GetHashCode();

    #endregion

    #region NewGuid

    /// <summary>
    /// Initialises a new instance of the ShortGuid class
    /// </summary>
    /// <returns></returns>
    public static ShortGuid NewGuid()
        => new ShortGuid(Guid.NewGuid());

    #endregion

    #region Encode

    /// <summary>
    /// Creates a new instance of a Guid using the String value,
    /// then returns the base64 encoded version of the Guid.
    /// </summary>
    /// <param name="value">An actual Guid String (i.e. not a ShortGuid)</param>
    /// <returns></returns>
    public static String Encode(String value)
        => ShortGuid.Encode(new Guid(value));

    /// <summary>
    /// Encodes the given Guid as a base64 String that is 22
    /// characters long.
    /// </summary>
    /// <param name="guid">The Guid to encode</param>
    /// <returns></returns>
    public static String Encode(Guid guid) {
      // NOTE:
      //   We encode GUIDs in their "diaplay" bytes order, not in their actual memory byte order.
      //   Otherwise we would have cross-platform compatibility issues, and get out of sync with
      //   numerous Java Script libraries that uses "display" byte order.
      var hexStr = guid.ToString("n");
      var rawBytes = Enumerable.Range(0, hexStr.Length)
          .Where(x => x % 2 == 0)
          .Select(x => Convert.ToByte(hexStr.Substring(x, 2), 16))
          .ToArray();

      var encoded = Convert.ToBase64String(rawBytes);
      encoded = encoded
          .Replace("/", "_")
          .Replace("+", "-");
      return encoded.Substring(0, 22);
    }

    #endregion

    #region Decode

    /// <summary>
    /// Decodes the given base64 String
    /// </summary>
    /// <param name="value">The base64 encoded String of a Guid</param>
    /// <returns>A new Guid</returns>
    public static Guid Decode(String value) {
      value = value
          .Replace("_", "/")
          .Replace("-", "+");
      var buf = Convert.FromBase64String(value + "==");

      // NOTE:
      //   We encode GUIDs in their "diaplay" bytes order, not in their actual memory byte order.
      //   Otherwise we would have cross-platform compatibility issues, and get out of sync with
      //   numerous Java Script libraries that uses "display" byte order.
      var hexStr = BitConverter.ToString(buf).Replace("-", String.Empty);
      return new Guid(hexStr);
    }

    #endregion

    #region Operators

    /// <summary>
    /// Determines if both ShortGuids have the same underlying
    /// Guid value.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool operator ==(ShortGuid x, ShortGuid y)
        => x.Guid == y.Guid;

    /// <summary>
    /// Determines if both ShortGuids do not have the
    /// same underlying Guid value.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool operator !=(ShortGuid x, ShortGuid y)
        => !(x == y);

    /// <summary>
    /// Implicitly converts the ShortGuid to it's String equivilent
    /// </summary>
    /// <param name="shortGuid"></param>
    /// <returns></returns>
    public static implicit operator String(ShortGuid shortGuid)
        => shortGuid.Value;

    /// <summary>
    /// Implicitly converts the ShortGuid to it's Guid equivilent
    /// </summary>
    /// <param name="shortGuid"></param>
    /// <returns></returns>
    public static implicit operator Guid(ShortGuid shortGuid)
        => shortGuid.Guid;

    /// <summary>
    /// Implicitly converts the String to a ShortGuid
    /// </summary>
    /// <param name="shortGuid"></param>
    /// <returns></returns>
    public static implicit operator ShortGuid(String shortGuid)
        => new ShortGuid(shortGuid);

    /// <summary>
    /// Implicitly converts the Guid to a ShortGuid
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public static implicit operator ShortGuid(Guid guid)
        => new ShortGuid(guid);

    #endregion
  }

  #endregion [ShortGuid class definition]

}
