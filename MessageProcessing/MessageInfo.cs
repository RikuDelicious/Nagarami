using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageProcessing
{
    public class MessageInfo
    {
        public MessageInfo(string tags, string prefix, string command, string parameters)
        {
            TagsRaw = tags;
            PrefixRaw = prefix;
            CommandRaw = command;
            ParametersRaw = parameters;

            switch (CommandRaw)
            {
                case "PRIVMSG":
                    MessageType = MessageType.PRIVMSG;
                    string[] parameterValues = MessageParser.DividePRIVMSGParameters(ParametersRaw);
                    Parameters["channel"] = parameterValues[0];
                    Parameters["chat-message"] = parameterValues[1];
                    Tags = MessageParser.ParseTagsIntoDictionary(TagsRaw);
                    break;
                case "PING":
                    MessageType = MessageType.PING;
                    Parameters["text"] = MessageParser.GetPINGText(ParametersRaw);
                    break;
                default:
                    MessageType = MessageType.NOTSUPPORTED;
                    break;
            }
        }

        public string TagsRaw { get; } = string.Empty;
        public string PrefixRaw { get; } = string.Empty;
        public string CommandRaw { get; } = string.Empty;
        public string ParametersRaw { get; } = string.Empty;
        public MessageType MessageType { get; }
        public Dictionary<string, string> Parameters { get; } = new Dictionary<string, string>();
        public Dictionary<string, string> Tags { get; } = new Dictionary<string, string>();
    }
}
