using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementHunting.Models
{
    public class Achievement
    {
        public bool Success { get; set; }
        public int GamePercent { get; set; }
        public int Unlocked { get; set; }
        public int Total { get; set; }
        public List<GameAchievement> Achievements { get; set; }
    }
}
