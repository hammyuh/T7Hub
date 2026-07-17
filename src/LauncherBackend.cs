using System.Diagnostics;
using System.IO;

namespace T7_Hub
{
    public class LauncherBackend
    {
        static Config config = ConfigManager.Load();
        static string bo3Path = config.GamePath;
        public static void LaunchStock()
        {
            Process.Start(new ProcessStartInfo { FileName = "steam://rungameid/311210", UseShellExecute = true });
        }
        public static void LaunchBOIII()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(config.GamePath, "boiii.exe"),
                WorkingDirectory = config.GamePath
            });
        }
        public static void LaunchT7x()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(config.GamePath, "t7x.exe"),
                WorkingDirectory = config.GamePath
            });
        }

    }
}
