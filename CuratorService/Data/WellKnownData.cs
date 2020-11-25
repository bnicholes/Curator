using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace curator.Data
{
    public class WellKnownData
    {
        public const string AppSettings = "appsettings";

        public const string PluginInfoClassName = "PluginDescriptor";

        public const string CuratorServiceName = "CuratorService";

        public const string ManifestPattern = "Manifest.json";
        public const string DllPattern = "*.dll";

        public const string PluginDirName = "ExternalPlugins";
        public const string PluginStageName = "PluginStaging";

        public static readonly string ProgramDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), CuratorServiceName);
        public static readonly string ServiceDirPath = Path.GetDirectoryName(
            System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) ?
                Assembly.GetExecutingAssembly().Location : Process.GetCurrentProcess().MainModule.FileName);
        public static readonly string PluginDirPath = Path.Combine(ProgramDataPath, PluginDirName);
        public static readonly string PluginStageDirPath = Path.Combine(ProgramDataPath, PluginStageName);

        public static string DevOpsServiceVersion()
        {
            var assem = Assembly.GetAssembly(typeof(CuratorService));
            var version = assem.GetName().Version;
            var buildType = assem.GetCustomAttributes(false).OfType<DebuggableAttribute>().Any(da => da.IsJITTrackingEnabled) ? "Debug" : "Release";
            return $"{buildType}-{version}";
        }
    }
}
