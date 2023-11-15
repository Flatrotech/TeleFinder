using Newtonsoft.Json.Linq;
using System.Collections;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        //string path = @"..\..\..\..\Debug\config.json";
        //string json = File.ReadAllText(path);
        //Console.WriteLine(JObject.Parse(json).SelectToken("$.Telegram.bot_token"));

        var bot_token = System.Environment.GetEnvironmentVariable("BotToken");

        Console.WriteLine(bot_token);
    }
}
