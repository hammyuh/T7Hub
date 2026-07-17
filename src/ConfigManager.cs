using System.IO;
using System.Text.Json;

namespace T7_Hub
{
    public static class ConfigManager
    {
        private static readonly string configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "T7 Hub",
            "config.json"
        );

        private static readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        public static Config Load()
        {
            if (!File.Exists(configPath))
            {
                Config config = new Config();
                Save(config);
                return config;
            }

            try
            {
                string json = File.ReadAllText(configPath);
                Config? config = JsonSerializer.Deserialize<Config>(json);

                if (config != null)
                    return config;
            }
            catch (JsonException)
            {
            }
            catch (IOException)
            {
            }

            Config newConfig = new Config();
            Save(newConfig);
            return newConfig;
        }

        public static void Save(Config config)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(configPath)!);

            string json = JsonSerializer.Serialize(config, options);

            File.WriteAllText(configPath, json);
        }

        internal static void Save(bool hasCompletedSetup)
        {
            throw new NotImplementedException();
        }
    }
}