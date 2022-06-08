using Websocket.Client;
using System.Reactive.Linq;
using MessageProcessing;

namespace TwitchIRC
{
    public static class ChatClient
    {
        public static event Action<MessageInfo>? PRIVMSGReceived;

        private static void ConnectAsync(string channel)
        {
            var exitEvent = new ManualResetEvent(false);
            var url = new Uri("wss://irc-ws.chat.twitch.tv:443");

            using (var client = new WebsocketClient(url))
            {
                client.ReconnectTimeout = TimeSpan.FromSeconds(30);
                
                client.MessageReceived.Subscribe(msg =>
                {
                    string[] messages = MessageParser.DivideMessages(msg.Text);

                    foreach (string message in messages)
                    {
                        MessageInfo messageInfo = MessageParser.CreateMessageInfo(message);

                        switch (messageInfo.MessageType)
                        {
                            case MessageType.PING:
                                string replyText = "PONG :" + messageInfo.Parameters["text"];
                                client.Send(replyText);
                                break;
                            case MessageType.PRIVMSG:
                                PRIVMSGReceived?.Invoke(messageInfo);
                                break;
                        }
                    }
                });

                client.Start();

                client.Send("CAP REQ :twitch.tv/commands twitch.tv/tags");
                client.Send("PASS oauth:THISISPASS");

                Random random = new Random();
                client.Send("NICK justinfan" + random.Next(10000));
                client.Send("JOIN #" + channel);

                exitEvent.WaitOne();
            }
        }

        public static async void ConnectToChat(string channel)
        {
            await Task.Run(() => { ConnectAsync(channel); });
        }
    }
}