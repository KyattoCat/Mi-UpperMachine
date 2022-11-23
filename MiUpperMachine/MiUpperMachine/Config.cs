using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;

namespace MiUpperMachine
{
    public class Config
    {
        public string? LastOpenDirectory;
        public string? ImageDLLPath;
        public string? EntryMethodName;
    }

    public static class ConfigManager
    {
        public const string DEFAULT_PATH = @".\GlobalConfig.json";

        public static Config globalConfig;

        static ConfigManager()
        {
            Config? config = LoadConfig();
            globalConfig = config ?? new Config();
        }

        public static Config? LoadConfig(string path = DEFAULT_PATH)
        {
            string configJson;
            try
            {
                configJson = File.ReadAllText(path);
            }
            catch (Exception)
            {
                MessageBox.Show($"{path}不存在");
                return null;
            }

            return JsonConvert.DeserializeObject<Config>(configJson);
        }

        public static void SaveGlobalConfig()
        {
            if (globalConfig != null)
            {
                SaveConfig(globalConfig, DEFAULT_PATH);
            }
        }

        public static void SaveConfig(Config config, string path = DEFAULT_PATH)
        {
            string configJson = JsonConvert.SerializeObject(config);
            try
            {
                File.WriteAllText(path, configJson);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
    }
}
