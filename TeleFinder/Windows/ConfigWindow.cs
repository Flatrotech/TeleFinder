using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using TeleFinder.Delivery;
using TeleFinder.Util;

namespace TeleFinder.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration configuration;
    
    public ConfigWindow(Plugin plugin) : base(
        "TeleFinder Configuration",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.AlwaysAutoResize)
    {
        configuration = Plugin.Configuration;
    }

    public void Dispose() { }

    private readonly TimedBool notifSentMessageTimer = new(3.0f);

    public override void Draw()
    {
        {
            var cfg = configuration.TelegramUsername;
            if (ImGui.InputText("Telegram Username", ref cfg, 2048u))
            {
                configuration.TelegramUsername = cfg;
            }
        }
        {
            var cfg = configuration.EnableForDutyPops;
            if (ImGui.Checkbox("Send message for duty pop?", ref cfg))
            {
                configuration.EnableForDutyPops = cfg;
            }
        }

        if (ImGui.Button("Send test notification"))
        {
            notifSentMessageTimer.Start();
            TelegramDelivery.Deliver("Test notification", 
                                     "If you received this, PushyFinder is configured correctly.");
        }

        if (notifSentMessageTimer.Value)
        {
            ImGui.SameLine();
            ImGui.Text("Notification sent!");
        }

        {
            var cfg = configuration.IgnoreAfkStatus;
            if (ImGui.Checkbox("Ignore AFK status and always notify", ref cfg))
            {
                configuration.IgnoreAfkStatus = cfg;
            }
        }

        if (!configuration.IgnoreAfkStatus)
        {
            if (!CharacterUtil.IsClientAfk())
            {
                var red = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
                ImGui.TextColored(red, "This plugin will only function while your client is AFK (/afk, red icon)!");

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("The reasoning for this is that if you are not AFK, you are assumed to");
                    ImGui.Text("be at your computer, and ready to respond to a join or a duty pop.");
                    ImGui.Text("Notifications would be bothersome, so they are disabled.");
                    ImGui.EndTooltip();
                }
            }
            else
            {
                var green = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
                ImGui.TextColored(green, "You are AFK. The plugin is active and notifications will be served.");
            }
        }

        if (ImGui.Button("Save and close"))
        {
            configuration.Save();
            IsOpen = false;
        }
    }
}
