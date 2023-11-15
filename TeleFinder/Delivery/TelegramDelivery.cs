using System;
using System.Threading.Tasks;
using Dalamud.Logging;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;

namespace TeleFinder.Delivery;

public static class TelegramDelivery
{
    public static readonly string TELEGRAM_API = $"https://api.telegram.org/bot{Environment.GetEnvironmentVariable("BotToken")}";


    public static void Deliver(string title, string text = "")
    {
        if (Plugin.Configuration.TelegramUsername.Length == 0) return;
        
        Task.Run(() => DeliverAsync(title, text));
    }

    private static async void DeliverAsync(string title, string text)
    {
        // first we need to get the chat_id of the user
        string chatId = null;

        try
        {
            var response = TELEGRAM_API
                .AppendPathSegment("getUpdates")
                .GetStringAsync()
                .Result;

            var result = JObject.Parse(response).SelectToken("$.result");
            foreach (var r in result) 
            {
                var usernameFromJson = r.SelectToken("$.message.from.username");
                if (usernameFromJson != null && usernameFromJson.ToString().ToLower() == Plugin.Configuration.TelegramUsername.ToLower())
                {
                    chatId = (string)r.SelectToken("$.message.chat.id");
                    break;
                }
            }

            PluginLog.Debug($"Telegram chat_id: {chatId}");
        }
        catch (FlurlHttpException e)
        {
            PluginLog.Error($"Failed to make Telegram req: '{e.Message}'");
            PluginLog.Error($"{e.StackTrace}");
        }

        try
        {
            // params are chat_id and text
            await TELEGRAM_API
                .AppendPathSegment("sendMessage")
                .SetQueryParams(new {chat_id = chatId, text = $"{title}\n{text}"})
                .GetAsync();
        }
        catch (FlurlHttpException e)
        {
            PluginLog.Error($"Failed to make Pushover req: '{e.Message}'");
            PluginLog.Error($"{e.StackTrace}");
        }
    }
}
