using MauiApp1.Helpers;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Editor.Text = TgHelper.GetTgBotToken();

        if (!string.IsNullOrWhiteSpace(TgHelper.GetChatId()))
        {
            UpdatesLabel.Text = $"Chat ID: {TgHelper.GetChatId()}. Ready to intercept sms and send it to the tg bot.";
        }

        SenderFilterEntry.Text = TgHelper.GetSenderFilter();
        MessageFilterEntry.Text = TgHelper.GetMessageFilter();
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        var status = await PermissionsHelper.CheckAndRequestSMSPermissionAsync();
        PermissionBtn.Text = status.ToString();
    }

    private async void ChatIdBtn_OnClicked(object? sender, EventArgs e)
    {
        var chatId = await TgHelper.CheckChatIdAsync();

        if (!string.IsNullOrEmpty(chatId))
        {
            UpdatesLabel.Text = $"Chat ID: {chatId}. Ready to intercept sms and send it to the tg bot.";
            TgHelper.SetChatId(chatId);
        }
        else
        {
            UpdatesLabel.Text =
                $"Has no messages or chat id detected. Try again later after writing /start inside tg bot.";
        }
    }

    private void Editor_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not Editor editor) return;
        TgHelper.SetTgBotToken(editor.Text);
        ChatIdBtn.IsEnabled = !string.IsNullOrWhiteSpace(editor.Text);
    }

    private void SenderFilterEntry_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry) return;
        TgHelper.SetSenderFilter(entry.Text);
    }

    private void MessageFilterEntry_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry) return;
        TgHelper.SetMessageFilter(entry.Text);
    }
}