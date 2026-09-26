using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementHunting.Models
{
    public class User
    {
        public string SteamID { get; set; }
        public string Nickname { get; set; }
        public string ProfileImageUrl { get; set; }
        public string ProfileFullImageUrl { get; set; }
    }
}
