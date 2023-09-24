using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TubeLoadr.Models;

namespace TubeLoadr.Services.UserSettings
{
    internal class UserSettings
    {
        public UserSettings()
        {
            if (!File.Exists($"{DataStorage.UserDataFolder}\\usersettings.json"))
            {
                Dictionary<string, string> UserSettings = new Dictionary<string, string>();
                var json = JsonSerializer.Serialize(UserSettings);
                File.WriteAllText($"{DataStorage.UserDataFolder}\\usersettings.json", json);
            }
        }
        public void ChangeSetting(string key, string value)
        {
            Dictionary<string, string> _data = LoadJson();
            if (_data.ContainsKey(key))
            {
                _data[key] = value;
                SaveChanges(_data);
                return;
            }
            _data.Add(key, value);
            SaveChanges(_data);
        }

        public string GetSetting(string key)
        {
            Dictionary<string, string> _data = LoadJson();
            if (_data.ContainsKey(key))
            {
                return _data[key];
            }
            return string.Empty;
        }

        private Dictionary<string, string> LoadJson()
        {
            using (StreamReader r = new StreamReader($"{DataStorage.UserDataFolder}\\usersettings.json"))
            {
                string json = r.ReadToEnd();
                Dictionary<string, string> items = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                return items;
            }
        }

        private void SaveChanges(Dictionary<string, string> data)
        {
            string json = JsonSerializer.Serialize(data);
            File.WriteAllText($"{DataStorage.UserDataFolder}\\usersettings.json", json);
        }
    }
}
