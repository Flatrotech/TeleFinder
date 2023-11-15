using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace TeleFinder
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 1;
        public string TelegramUsername { get; set; } = "";
        public bool EnableForDutyPops { get; set; } = true;
        public bool IgnoreAfkStatus { get; set; } = false;

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private DalamudPluginInterface? PluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.PluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.PluginInterface!.SavePluginConfig(this);
        }
    }
}
