using Newtonsoft.Json.Linq;

namespace MauiApp1.Helpers;

public static class TgHelper
{
    private const string TgBotTokenKey = nameof(TgBotTokenKey);
    private const string ChatIdKey = nameof(ChatIdKey);
    private const string SenderFilter = nameof(SenderFilter);
    private const string MessageFilter = nameof(MessageFilter);

    public static async Task SendToTelegramAsync(string sender, string messageBody)
    {
        var url = $"https://api.telegram.org/bot{GetTgBotToken()}/sendMessage";

        var compareSenderFilterCounter = 0;
        var compareMessageFilterCounter = 0;

        var senderFilters = GetSenderFilter().Split(";");

        foreach (var filter in senderFilters)
        {
            if ((!string.IsNullOrWhiteSpace(filter) && sender.Contains(filter, StringComparison.OrdinalIgnoreCase)) ||
                (string.IsNullOrWhiteSpace(filter) && senderFilters.Length == 1)) compareSenderFilterCounter++;
        }

        var messageBodyFilters = GetMessageFilter().Split(";");

        foreach (var filter in messageBodyFilters)
        {
            if ((!string.IsNullOrWhiteSpace(filter) && messageBody.Contains(filter, StringComparison.OrdinalIgnoreCase)) ||
                (string.IsNullOrWhiteSpace(filter) && messageBodyFilters.Length == 1)) compareMessageFilterCounter++;
        }

        if (compareSenderFilterCounter == 0 || compareMessageFilterCounter == 0) return;

        var client = new HttpClient();
        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("chat_id", GetChatId()),
            new KeyValuePair<string, string>("text", $"From: {sender}\nMessage: {messageBody}")
        ]);

        await client.PostAsync(url, content);
    }

    public static async Task<string> CheckChatIdAsync()
    {
        try
        {
            var url = $"https://api.telegram.org/bot{GetTgBotToken()}/getUpdates";

            using var client = new HttpClient();
            var response = await client.GetStringAsync(url);
            var json = JObject.Parse(response);

            return json["result"]?[0]?["message"]?["chat"]?["id"]?.ToString();
        }
        catch (Exception e)
        {
            return string.Empty;
        }
    }

    public static void SetTgBotToken(string tgBotToken)
    {
        Preferences.Default.Set(TgBotTokenKey, tgBotToken);
    }

    public static string GetTgBotToken()
    {
        return Preferences.Default.Get(TgBotTokenKey, string.Empty);
    }

    public static void SetChatId(string chatId)
    {
        Preferences.Default.Set(ChatIdKey, chatId);
    }

    public static string GetChatId()
    {
        return Preferences.Default.Get(ChatIdKey, string.Empty);
    }

    public static void SetSenderFilter(string senderFilter)
    {
        Preferences.Default.Set(SenderFilter, senderFilter);
    }

    public static string GetSenderFilter()
    {
        return Preferences.Default.Get(SenderFilter, string.Empty);
    }

    public static void SetMessageFilter(string messageFilter)
    {
        Preferences.Default.Set(MessageFilter, messageFilter);
    }

    public static string GetMessageFilter()
    {
        return Preferences.Default.Get(MessageFilter, string.Empty);
    }
}