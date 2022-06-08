using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MessageProcessing
{
    public class MessageParser
    {
        public static string[] DivideMessageIntoPrimaryParts(string rawMessage)
        {
            string tags = "", prefix = "", command = "", parameters = "";

            // commandしかない場合
            if (rawMessage.IndexOf(' ') == -1)
            {
                command = rawMessage.Trim();
                return new string[] { tags, prefix, command, parameters };
            }
            
            string tagsOrPrefixOrCommand = rawMessage[0..rawMessage.IndexOf(' ')];
            rawMessage = rawMessage.Remove(0, tagsOrPrefixOrCommand.Length + 1);

            if (tagsOrPrefixOrCommand[0] == '@')
            {
                tags = tagsOrPrefixOrCommand.Trim();

                // commandしかない場合
                if (rawMessage.IndexOf(' ') == -1)
                {
                    command = rawMessage.Trim();
                    return new string[] { tags, prefix, command, parameters };
                }

                string prefixOrCommand = rawMessage[0..rawMessage.IndexOf(' ')];
                rawMessage = rawMessage.Remove(0, prefixOrCommand.Length + 1);

                if (prefixOrCommand[0] == ':')
                {
                    prefix = prefixOrCommand.Trim();

                    if (rawMessage.IndexOf(' ') == -1)
                    {
                        command = rawMessage.Trim();
                        return new string[] { tags, prefix, command, parameters };
                    }
                    else
                    {
                        command = rawMessage[0..rawMessage.IndexOf(' ')].Trim();
                        parameters = rawMessage.Remove(0, rawMessage[0..rawMessage.IndexOf(' ')].Length + 1);
                        return new string[] { tags, prefix, command, parameters };
                    }
                }
                else
                {
                    command = prefixOrCommand.Trim();
                    parameters = rawMessage.Trim();
                    return new string[] { tags, prefix, command, parameters };
                }
            }
            else if (tagsOrPrefixOrCommand[0] == ':')
            {
                prefix = tagsOrPrefixOrCommand.Trim();

                if (rawMessage.IndexOf(' ') == -1)
                {
                    command = rawMessage.Trim();
                    return new string[] { tags, prefix, command, parameters };
                }
                else
                {
                    command = rawMessage[0..rawMessage.IndexOf(' ')].Trim();
                    parameters = rawMessage.Remove(0, rawMessage[0..rawMessage.IndexOf(' ')].Length + 1);
                    return new string[] { tags, prefix, command, parameters };
                }
            }
            else
            {
                command = tagsOrPrefixOrCommand.Trim();
                parameters = rawMessage.Trim();
                return new string[] { tags, prefix, command, parameters };
            }
        }

        internal static string GetPINGText(string parametersRaw)
        {
            if (parametersRaw.IndexOf(':') != -1)
            {
                int start = parametersRaw.IndexOf(':') + 1;
                int end = parametersRaw.Length;
                return parametersRaw[start..end];
            }
            else
            {
                return string.Empty;
            }
        }

        public static string[] DivideMessages(string rawMessages)
        {
            return rawMessages.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] DividePRIVMSGParameters(string rawParameters)
        {
            string channelName = "", chatMessage = "";
            int index = 0;
            if (rawParameters.IndexOf('#', index) != -1)
            {
                index = rawParameters.IndexOf('#', index) + 1;
                int endIndex = rawParameters.IndexOf(' ', index);
                channelName = rawParameters[index..endIndex].Trim();
                index = endIndex + 1;

                if (rawParameters.IndexOf(':', index) != -1)
                {
                    index = rawParameters.IndexOf(':', index) + 1;
                    endIndex = rawParameters.Length;
                    chatMessage = rawParameters[index..endIndex].Trim();
                }
            }

            return new string[] { channelName, chatMessage };
        }

        public static Dictionary<string, string> ParseTagsIntoDictionary(string rawTags)
        {
            Dictionary<string, string> tags = new Dictionary<string, string>();

            if (rawTags[0] == '@')
            {
                rawTags = rawTags.Remove(0, 1);
            }

            string[] dividedTags = rawTags.Split(';', StringSplitOptions.TrimEntries);
            foreach (string tag in dividedTags)
            {
                string[] tagNameAndValue = tag.Split('=', StringSplitOptions.TrimEntries);
                tags.Add(tagNameAndValue[0], tagNameAndValue[1]);
            }

            return tags;
        }

        public static MessageInfo CreateMessageInfo(string rawMessage)
        {
            string[] rawMessageParts = DivideMessageIntoPrimaryParts(rawMessage);
            MessageInfo messageInfo = new MessageInfo(rawMessageParts[0],
                rawMessageParts[1], rawMessageParts[2], rawMessageParts[3]);
            return messageInfo;
        }
    }
}
