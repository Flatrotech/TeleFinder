using Newtonsoft.Json.Linq;

class Program
{
    static void Main(string[] args)
    {
        string path = @"..\..\..\..\Debug\config.json";
        string json = File.ReadAllText(path);
        Console.WriteLine(JObject.Parse(json).SelectToken("$.Telegram.bot_token"));
    }
}
