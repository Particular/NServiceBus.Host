namespace NServiceBus
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    class FileVersionRetriever
    {
        // Retrieves a semver compliant version from a <see cref="Type"/>s Assembly.
        public static string GetFileVersion(Type type)
        {
            var assembly = type.Assembly;
            if (!string.IsNullOrEmpty(assembly.Location))
            {
                var fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);

                return new Version(fileVersion.FileMajorPart, fileVersion.FileMinorPart, fileVersion.FileBuildPart).ToString(3);
            }

            var customAttributes = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);

            if (customAttributes.Length >= 1)
            {
                var fileVersion = (AssemblyFileVersionAttribute)customAttributes[0];
                Version version;
                if (Version.TryParse(fileVersion.Version, out version))
                {
                    return version.ToString(3);
                }
            }

            return assembly.GetName().Version.ToString(3);
        }
    }
}
