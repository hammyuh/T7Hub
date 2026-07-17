using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace T7_Hub
{
    public static class ExeChecker
    {
        public static string GetHash(string file)
        {
            using SHA256 sha = SHA256.Create();

            using FileStream stream = File.OpenRead(file);

            byte[] hash = sha.ComputeHash(stream);

            return Convert.ToHexString(hash);
        }


        public static string CheckBO3(string exePath)
        {
            if (!File.Exists(exePath))
                return "Missing";

            string hash = GetHash(exePath);

            string? json = HashUpdater.GetHashes();

            if (json == null)
                return "Unknown";

            using JsonDocument doc = JsonDocument.Parse(json);

            var bo3 = doc.RootElement.GetProperty("blackops3");


            foreach (var oldHash in bo3.GetProperty("old").EnumerateArray())
            {
                if (hash == oldHash.GetString())
                    return "Old";
            }


            foreach (var newHash in bo3.GetProperty("new").EnumerateArray())
            {
                if (hash == newHash.GetString())
                    return "New";
            }


            return "Unknown";
        }
    }
}