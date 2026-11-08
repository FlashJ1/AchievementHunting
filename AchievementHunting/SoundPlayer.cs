using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementHunting
{
    public static class SoundPlayer
    {
        private static WaveOutEvent? player;
        private static AudioFileReader? audioFile;

        public static void Play(string filePath)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sfx", filePath);
            if (!File.Exists(path))
                return;
            Stop();
            audioFile = new AudioFileReader(path);
            player = new WaveOutEvent();
            player.Init(audioFile);
            player.Play();
        }
        public static void Stop()
        {
            player?.Stop();
            player?.Dispose();
            player = null;
            audioFile?.Dispose();
            audioFile = null;
        }
        public static void PlayByRarity(string globalPercent)
        {
            double percent = double.TryParse(globalPercent, CultureInfo.InvariantCulture, out double result) ? result : 0;
            if (percent <= 5)
                Play("legendary.mp3");
            else if (percent > 5 && percent <= 10)
                Play("epic.mp3");
            else if (percent > 10 && percent <= 50)
                Play("rare.mp3");
        }
    }
}
