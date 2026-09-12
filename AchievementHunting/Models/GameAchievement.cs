using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AchievementHunting.Models
{
    public class GameAchievement
    {
        public string Name { get; set; }
        public string? Desc { get; set; }
        public string APIName { get; set; }
        public string GlobalPercent { get; set; }
        public string IconURL { get; set; }
        public string IconGrayURL { get; set; }
        [JsonIgnore]
        public Image Icon { get; set; }
        public bool Achieved { get; set; }
        public bool Hidden { get; set; }
    }
}
