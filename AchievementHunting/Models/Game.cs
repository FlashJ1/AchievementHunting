using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AchievementHunting.Models
{
    public class Game
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public bool HasAchievements { get; set; }
        public int Percent { get; set; }
        public int UnlockedAchievements { get; set; }
        public int TotalAchievements { get; set; }
        public string ImgIconUrl { get; set; }
        [JsonIgnore]
        public Image? ImgIcon { get; set; }
        public List<GameAchievement> Achievements { get; set; }

    }
}
