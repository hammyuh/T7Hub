using Microsoft.Win32;
using System.IO;

namespace T7_Hub
{
    public class GameLocator
    {
        public static string? GetSteamPath()
        {
            return Registry.GetValue(
                @"HKEY_CURRENT_USER\Software\Valve\Steam",
                "SteamPath",
                null
            ) as string;
        }


        public static List<string> GetSteamLibraries()
        {
            List<string> libraries = new();

            string? steamPath = GetSteamPath();

            if (string.IsNullOrWhiteSpace(steamPath))
                return libraries;


            string libraryFile = Path.Combine(
                steamPath,
                "steamapps",
                "libraryfolders.vdf"
            );


            if (!File.Exists(libraryFile))
                return libraries;


            foreach (string line in File.ReadLines(libraryFile))
            {
                if (!line.Contains("\"path\""))
                    continue;


                string[] parts = line.Split('"');

                if (parts.Length < 4)
                    continue;


                string path = parts[3].Replace(@"\\", @"\");

                if (Directory.Exists(path))
                {
                    libraries.Add(path);
                }
            }


            return libraries;
        }


        public static string? FindBO3()
        {
            foreach (string library in GetSteamLibraries())
            {
                string bo3Path = Path.Combine(
                    library,
                    "steamapps",
                    "common",
                    "Call of Duty Black Ops III"
                );


                string exe = Path.Combine(
                    bo3Path,
                    "BlackOps3.exe"
                );


                if (File.Exists(exe))
                {
                    return bo3Path;
                }
            }


            return null;
        }
    }
}