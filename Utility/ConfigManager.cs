using System;
using System.IO;
using System.Text;
using System.Text.Json;
using gPadX.Models;

namespace gPadX.Utility {
    static class ConfigManager {
        readonly static string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
        static Lazy<ConfigModel> lazyConfig = new Lazy<ConfigModel>(() => {
            return TryLoad(out var config) ? config : new ConfigModel();
        }, false);

        public static ConfigModel Config {
            get { return lazyConfig.Value; }
        }

        static bool TryLoad(out ConfigModel config) {
            var success = false;
            config = null;

            try {
                var json = File.ReadAllText(configPath, Encoding.UTF8);
                config = JsonSerializer.Deserialize<ConfigModel>(json);
                success = config != null;
            } catch { }

            return success;
        }

        public static void Save() {
            var json = JsonSerializer.Serialize(Config);
            File.WriteAllText(configPath, json, Encoding.UTF8);
        }
    }
}
