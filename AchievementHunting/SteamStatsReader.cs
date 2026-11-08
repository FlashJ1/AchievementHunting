using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AchievementHunting
{
    public static class SteamStatsReader
    {
        public static Dictionary<(int Stat, int Bit), long>? ReadUnlocked(string path)
        {
            byte[]? bytes = ReadAllBytesShared(path);
            if (bytes == null)
                return null;
            Dictionary<string, object>? root = BinaryKv.TryParse(bytes);
            if (root == null)
                return null;
            if (!root.TryGetValue("cache", out object? cacheObj))
                return null;
            if (cacheObj is not Dictionary<string, object> cache)
                return null;
            var result = new Dictionary<(int Stat, int Bit), long>();
            foreach (KeyValuePair<string, object> entry in cache)
            {
                if (!int.TryParse(entry.Key, out int statId))
                    continue;
                if (entry.Value is not Dictionary<string, object> stat)
                    continue;
                if (!stat.TryGetValue("data", out object? dataObj) || dataObj is not int data)
                    continue;
                if (!stat.TryGetValue("AchievementTimes", out object? timesObj))
                    continue;
                if (timesObj is not Dictionary<string, object> times)
                    continue;
                uint bits = unchecked((uint)data);
                for (int bit = 0; bit < 32; bit++)
                {
                    if ((bits & (1u << bit)) == 0)
                        continue;
                    long time = 0;
                    if (times.TryGetValue(bit.ToString(), out object? t) && t is int seconds)
                        time = unchecked((uint)seconds);
                    result[(statId, bit)] = time;
                }
            }
            return result;
        }
        public static Dictionary<(int Stat, int Bit), string>? ReadSchema(string path)
        {
            byte[]? bytes = ReadAllBytesShared(path);
            if (bytes == null)
                return null;
            Dictionary<string, object>? root = BinaryKv.TryParse(bytes);
            if (root == null)
                return null;
            Dictionary<string, object>? app = null;
            foreach (object value in root.Values)
            {
                if (value is Dictionary<string, object> section)
                { 
                    app = section;
                    break;
                }
            }
            if (app == null)
                return null;
            if (!app.TryGetValue("stats", out object? statsObj))
                return null;
            if (statsObj is not Dictionary<string, object> stats)
                return null;
            var result = new Dictionary<(int Stat, int Bit), string>();
            foreach (KeyValuePair<string, object> statEntry in stats)
            {
                if (!int.TryParse(statEntry.Key, out int statId))
                    continue;
                if (statEntry.Value is not Dictionary<string, object> stat)
                    continue;
                if (!stat.TryGetValue("bits", out object? bitsObj))
                    continue;
                if (bitsObj is not Dictionary<string, object> bits)
                    continue;
                foreach (KeyValuePair<string, object> bitEntry in bits)
                {
                    if (!int.TryParse(bitEntry.Key, out int bit))
                        continue;
                    if (bitEntry.Value is not Dictionary<string, object> bitSection)
                        continue;
                    if (!bitSection.TryGetValue("name", out object? nameObj) || nameObj is not string name)
                        continue;
                    if (name.Length > 0)
                        result[(statId, bit)] = name;
                }
            }
            return result;
        }
        private static byte[]? ReadAllBytesShared(string path)
        {
            try
            {
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                byte[] buffer = new byte[fs.Length];
                int read = 0;
                while (read < buffer.Length)
                {
                    int n = fs.Read(buffer, read, buffer.Length - read);
                    if (n <= 0)
                        break;
                    read += n;
                }
                if (read == buffer.Length)
                    return buffer;
                return buffer[..read];
            }
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }
    }
    internal static class BinaryKv
    {
        public static Dictionary<string, object>? TryParse(byte[] data)
        {
            try
            {
                int pos = 0;
                return ReadSection(data, ref pos);
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static Dictionary<string, object> ReadSection(byte[] d, ref int pos)
        {
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            while (true)
            {
                if (pos >= d.Length)
                    throw new InvalidDataException("Unexpected end of data.");
                byte type = d[pos++];
                if (type == 8)
                    return result;
                string name = ReadCString(d, ref pos);
                switch (type)
                {
                    case 0:
                        result[name] = ReadSection(d, ref pos);
                        break;
                    case 1:
                        result[name] = ReadCString(d, ref pos);
                        break;
                    case 2:
                    case 4:
                    case 6:
                        EnsureAvailable(d, pos, 4);
                        result[name] = BitConverter.ToInt32(d, pos);
                        pos += 4;
                        break;
                    case 3:
                        EnsureAvailable(d, pos, 4);
                        result[name] = BitConverter.ToSingle(d, pos);
                        pos += 4;
                        break;
                    case 7:
                        EnsureAvailable(d, pos, 8);
                        result[name] = BitConverter.ToUInt64(d, pos);
                        pos += 8;
                        break;
                    case 10:
                        EnsureAvailable(d, pos, 8);
                        result[name] = BitConverter.ToInt64(d, pos);
                        pos += 8;
                        break;
                    default:
                        throw new InvalidDataException($"Unsupported KeyValues type {type}.");
                }
            }
        }
        private static void EnsureAvailable(byte[] d, int pos, int count)
        {
            if (pos + count > d.Length)
                throw new InvalidDataException("Unexpected end of data.");
        }
        private static string ReadCString(byte[] d, ref int pos)
        {
            int end = Array.IndexOf(d, (byte)0, pos);
            if (end < 0)
                throw new InvalidDataException("Unterminated string.");
            string s = Encoding.UTF8.GetString(d, pos, end - pos);
            pos = end + 1;
            return s;
        }
    }
}