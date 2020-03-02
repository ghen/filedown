using System;
using System.Linq;
using System.Reflection;

namespace FileDownload.Extensions {

  public static class ReflectionExtensions {

    #region VersionString

    /// <summary>
    /// Reports assembly name and version.
    /// </summary>
    /// <returns>Assembly name and version.</returns>
    public static String VersionString(this Assembly assembly) {
      if (assembly == null) return String.Empty;

      var version = assembly.GetName().Version.ToString(3);
      var title = ((AssemblyTitleAttribute)assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true)[0])
          .Title;

      var res = $"{title} v{version}";
      return res;
    }

    #endregion VersionString

    #region BuildString

    /// <summary>
    /// Reports assembly build version.
    /// </summary>
    /// <returns>Assembly build version.</returns>
    public static String BuildString(this Assembly assembly) {
      if (assembly == null) return String.Empty;

      var version = ((AssemblyInformationalVersionAttribute)assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), true)
          .FirstOrDefault())?.InformationalVersion ?? assembly.GetName().Version.ToString();

      var conf = (String)null;
#if DEBUG
      conf = ((AssemblyConfigurationAttribute)assembly.GetCustomAttributes(
          typeof(AssemblyConfigurationAttribute), true)[0]).Configuration;
#endif

      var res = $"v{version}{(!String.IsNullOrEmpty(conf) ? $" ({conf})" : String.Empty)}";
      return res;
    }

    #endregion BuildString

  }

}
