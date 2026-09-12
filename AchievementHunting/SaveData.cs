using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using AchievementHunting.Models;

namespace AchievementHunting
{
    public static class SaveData
    {
        private static readonly string FilePath = Path.Combine(Application.StartupPath, "games.json");
        private static readonly string PlayerFilePath = Path.Combine(Application.StartupPath, "player.json");
        public static void Save(List<Game> games)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(games, options);
            File.WriteAllText(FilePath, json);
        }

        public static void Save(User user)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(user, options);
            File.WriteAllText(PlayerFilePath, json);
        }

        public static List<Game> Load()
        {
            if (!File.Exists(FilePath)) return new List<Game>();
            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<Game>>(json) ?? new List<Game>();
        }

        public static User LoadUser()
        {
            if (!File.Exists(PlayerFilePath)) return new User();
            string json = File.ReadAllText(PlayerFilePath);
            return JsonSerializer.Deserialize<User>(json) ?? new User();
        }

        public static bool IsJSONNull()
        {
            if (!File.Exists(FilePath)) return true;
            string json = File.ReadAllText(FilePath);
            return string.IsNullOrWhiteSpace(json);
        }
    }
}
