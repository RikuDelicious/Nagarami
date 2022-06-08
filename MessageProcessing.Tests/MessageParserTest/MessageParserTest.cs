using FluentAssertions;
using System.Xml;

namespace MessageProcessing.Tests
{
    [TestClass]
    public class MessageParserTest
    {
        [TestMethod]
        public void �����̃��b�Z�[�W��1���ɕ�������()
        {
            /*
             * RFC1459 2.3 Messages ���
             * �e���b�Z�[�W�͕K��CR-LF(Carriage Return - Line Feed)�ŏI����Ă��܂��B
             * CR-LF���e���b�Z�[�W�̋�؂�Ƃ݂Ȃ����Ƃ��ł��܂��B
             */
            {
                string rawMessages = ":foo!foo@foo.tmi.twitch.tv JOIN #bar\r\n:foo.tmi.twitch.tv 353 foo = #bar :foo\r\n:foo.tmi.twitch.tv 366 foo #bar :End of /NAMES list\r\n@badge-info=;badges=moderator/1;color=;display-name=foo;emote-sets=0,300374282;mod=1;subscriber=0;user-type=mod :tmi.twitch.tv USERSTATE #bar\r\n@emote-only=0;followers-only=-1;r9k=0;rituals=0;room-id=12345678;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #bar\r\n:foo!foo@foo.tmi.twitch.tv PART #bar\r\n";

                string[] actualDividedMessages = MessageParser.DivideMessages(rawMessages);

                string[] expectedDividedMessages = new string[]
                {
                    ":foo!foo@foo.tmi.twitch.tv JOIN #bar",
                    ":foo.tmi.twitch.tv 353 foo = #bar :foo",
                    ":foo.tmi.twitch.tv 366 foo #bar :End of /NAMES list",
                    "@badge-info=;badges=moderator/1;color=;display-name=foo;emote-sets=0,300374282;mod=1;subscriber=0;user-type=mod :tmi.twitch.tv USERSTATE #bar",
                    "@emote-only=0;followers-only=-1;r9k=0;rituals=0;room-id=12345678;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #bar",
                    ":foo!foo@foo.tmi.twitch.tv PART #bar"
                };

                CollectionAssert.AreEqual(actualDividedMessages, expectedDividedMessages);
            }

            #region ���b�Z�[�W��1�̏ꍇ_1
            {
                string rawMessages = ":foo!foo@foo.tmi.twitch.tv JOIN #bar\r\n";
                string[] actualDividedMessages = MessageParser.DivideMessages(rawMessages);
                string[] expectedDividedMessages = new string[]
                {
                    ":foo!foo@foo.tmi.twitch.tv JOIN #bar"
                };
                CollectionAssert.AreEqual(actualDividedMessages, expectedDividedMessages);
            }
            #endregion

            #region ���b�Z�[�W��1�̏ꍇ_2
            {
                string rawMessages = ":foo!foo@foo.tmi.twitch.tv JOIN #bar";
                string[] actualDividedMessages = MessageParser.DivideMessages(rawMessages);
                string[] expectedDividedMessages = new string[]
                {
                    ":foo!foo@foo.tmi.twitch.tv JOIN #bar"
                };
                CollectionAssert.AreEqual(actualDividedMessages, expectedDividedMessages);
            }
            #endregion
        }

        [TestMethod]
        public void �P��̃��b�Z�[�W���e�p�[�g�ɕ�������_Prefix����_Tags�Ȃ�()
        {
            /*
             * RFC1459 2.3 Messages ���
             * IRC�̃��b�Z�[�W��prefix(optional),command,command parameters��3�̃p�[�g����\������Ă��܂��B
             * prefix, command, �eparameter��1�ȏ�̃X�y�[�X�ŋ�؂��Ă��܂��B
             * �^��BNF�ł͈ȉ��̂悤�ɒ�`����܂�
             * <message>  ::= [':' <prefix> <SPACE> ] <command> <params> <crlf>
             */

            #region CASE_1
            {
                string rawMessage = ":tmi.twitch.tv CAP * ACK :twitch.tv/commands twitch.tv/tags";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "",":tmi.twitch.tv","CAP","* ACK :twitch.tv/commands twitch.tv/tags"
                });
            }
            #endregion

            #region CASE_2
            {
                string rawMessage = ":tmi.twitch.tv 001 johndoe :Welcome, GLHF!";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "",":tmi.twitch.tv","001","johndoe :Welcome, GLHF!"
                });
            }
            #endregion

            #region CASE_3
            {
                string rawMessage = ":tmi.twitch.tv 002 johndoe :Your host is tmi.twitch.tv";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "",":tmi.twitch.tv","002","johndoe :Your host is tmi.twitch.tv"
                });
            }
            #endregion

            #region CASE_4
            {
                string rawMessage = ":tmi.twitch.tv 003 johndoe :This server is rather new";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "",":tmi.twitch.tv","003","johndoe :This server is rather new"
                });
            }
            #endregion

            #region CASE_5
            {
                string rawMessage = ":tmi.twitch.tv 004 johndoe :-";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "",":tmi.twitch.tv","004","johndoe :-"
                });
            }
            #endregion

            #region CASE_6
            {
                string rawMessage = ":tmi.twitch.tv 375 johndoe :-";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "",":tmi.twitch.tv","375","johndoe :-"
                });
            }
            #endregion

            #region CASE_7
            {
                string rawMessage = ":tmi.twitch.tv 372 johndoe :You are in a maze of twisty passages, all alike.";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "",":tmi.twitch.tv","372","johndoe :You are in a maze of twisty passages, all alike."
                });
            }
            #endregion

            #region CASE_8
            {
                string rawMessage = ":tmi.twitch.tv 376 johndoe :>";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "",":tmi.twitch.tv","376","johndoe :>"
                });
            }
            #endregion

            #region CASE_9
            {
                string rawMessage = ":johndoe!johndoe@johndoe.tmi.twitch.tv JOIN #fps_paul";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "",":johndoe!johndoe@johndoe.tmi.twitch.tv","JOIN","#fps_paul"
                });
            }
            #endregion

            #region CASE_10
            {
                string rawMessage = ":johndoe.tmi.twitch.tv 353 johndoe = #fps_paul :johndoe";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "",":johndoe.tmi.twitch.tv","353","johndoe = #fps_paul :johndoe"
                });
            }
            #endregion

            #region CASE_11
            {
                string rawMessage = ":johndoe.tmi.twitch.tv 366 johndoe #fps_paul :End of /NAMES list";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "",":johndoe.tmi.twitch.tv","366","johndoe #fps_paul :End of /NAMES list"
                });
            }
            #endregion

            #region CASE_12
            {
                string rawMessage = ":johndoe!johndoe@johndoe.tmi.twitch.tv PRIVMSG #fps_paul :�G�C���悷���I";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "",":johndoe!johndoe@johndoe.tmi.twitch.tv","PRIVMSG","#fps_paul :�G�C���悷���I"
                });
            }
            #endregion
        }

        [TestMethod]
        public void �P��̃��b�Z�[�W���e�p�[�g�ɕ�������_Prefix�Ȃ�_Tags�Ȃ�()
        {
            /*
             * RFC1459 2.3 Messages ���
             * IRC�̃��b�Z�[�W��prefix(optional),command,command parameters��3�̃p�[�g����\������Ă��܂��B
             * �^��BNF�ł͈ȉ��̂悤�ɒ�`����܂�
             * <message>  ::= [':' <prefix> <SPACE> ] <command> <params> <crlf>
             */

            // PING�ȊO���Ⴊ�����̂�prefix�����������^�����b�Z�[�W�Ńe�X�g���s��

            #region CASE_1
            {
                string rawMessage = "PING :tmi.twitch.tv";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "","","PING",":tmi.twitch.tv"
                });
            }
            #endregion

            #region CASE_2
            {
                string rawMessage = "CAP * ACK :twitch.tv/commands twitch.tv/tags";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "","","CAP","* ACK :twitch.tv/commands twitch.tv/tags"
                });
            }
            #endregion

            #region CASE_3
            {
                string rawMessage = "001 johndoe :Welcome, GLHF!";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "","","001","johndoe :Welcome, GLHF!"
                });
            }
            #endregion

            #region CASE_4
            {
                string rawMessage = "JOIN #fps_paul";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "","","JOIN","#fps_paul"
                });
            }
            #endregion

            #region CASE_5
            {
                string rawMessage = "353 johndoe = #fps_paul :johndoe";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "","","353","johndoe = #fps_paul :johndoe"
                });
            }
            #endregion

            #region CASE_6
            {
                string rawMessage = "366 johndoe #fps_paul :End of /NAMES list";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "","","366","johndoe #fps_paul :End of /NAMES list"
                });
            }
            #endregion

            #region CASE_7
            {
                string rawMessage = "PRIVMSG #fps_paul :�G�C���ǂ����I";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "","","PRIVMSG","#fps_paul :�G�C���ǂ����I"
                });
            }
            #endregion
        }

        [TestMethod]
        public void �P��̃��b�Z�[�W���e�p�[�g�ɕ�������_Prefix����_Tags����()
        {
            /*
             * IRCv3 Message ���
             * IRC�̃��b�Z�[�W���g������prefix�̑O��tags��ǉ����܂�
             * �^��BNF�ł͈ȉ��̂悤�ɒ�`����܂�
             * <message> ::= ['@' <tags> <SPACE>] [':' <prefix> <SPACE> ] <command> [params] <crlf>
             */

            #region CASE_1
            {
                string rawMessage = "@emote-only=0;followers-only=10;r9k=1;rituals=0;room-id=12345678;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #fps_paul";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "@emote-only=0;followers-only=10;r9k=1;rituals=0;room-id=12345678;slow=0;subs-only=0",
                    ":tmi.twitch.tv", "ROOMSTATE", "#fps_paul"
                });
            }
            #endregion

            #region CASE_2
            {
                string rawMessage = "@badge-info=subscriber/17;badges=subscriber/12,premium/1;client-nonce=9e99ee99999c9e9ee9ee9e99e9e99999;color=;display-name=�W�����h�D;emotes=;first-msg=0;flags=;id=999eee99-eee9-99e9-99ee-e9eee9e9e9e9;mod=0;room-id=12345678;subscriber=1;tmi-sent-ts=1653573024309;turbo=0;user-id=987654321;user-type= :johndoe!johndoe@johndoe.tmi.twitch.tv PRIVMSG #fps_paul :�G�C���ǂ����I";
                string[] messageParts = MessageParser.DivideMessageIntoPrimaryParts(rawMessage);
                CollectionAssert.AreEqual(messageParts, new string[]
                {
                    "@badge-info=subscriber/17;badges=subscriber/12,premium/1;client-nonce=9e99ee99999c9e9ee9ee9e99e9e99999;color=;display-name=�W�����h�D;emotes=;first-msg=0;flags=;id=999eee99-eee9-99e9-99ee-e9eee9e9e9e9;mod=0;room-id=12345678;subscriber=1;tmi-sent-ts=1653573024309;turbo=0;user-id=987654321;user-type=",
                    ":johndoe!johndoe@johndoe.tmi.twitch.tv", "PRIVMSG", "#fps_paul :�G�C���ǂ����I"
                });
            }
            #endregion
        }

        // ���Ⴊ�����̂ň�U�ȗ�
        // [TestMethod]
        public void �P��̃��b�Z�[�W���e�p�[�g�ɕ�������_Prefix�Ȃ�_Tags����()
        {
            /*
             * IRCv3 Message ���
             * IRC�̃��b�Z�[�W��prefix�̑O��tags��t�����܂�
             * �^��BNF�ł͈ȉ��̂悤�ɒ�`����܂�
             * <message> ::= ['@' <tags> <SPACE>] [':' <prefix> <SPACE> ] <command> [params] <crlf>
             */
        }

        [TestMethod]
        public void TwitchIRC�T�[�o�[�����PRIVMSG���b�Z�[�W�̃p�����[�^�[�𕪊�����()
        {
            /*
             * PRIVMSG�ɂ���
             * �Q�l�Fhttps://dev.twitch.tv/docs/irc/send-receive-messages
             */

            #region CASE_1
            {
                string rawParameters = "#fps_paul :�G�C���ǂ����I";
                string[] parameters = MessageParser.DividePRIVMSGParameters(rawParameters);
                CollectionAssert.AreEqual(parameters, new string[]
                {
                    "fps_paul",
                    "�G�C���ǂ����I"
                });
            }
            #endregion
        }

        [TestMethod]
        public void �P��̃��b�Z�[�W����ParsedMessage�I�u�W�F�N�g���쐬����()
        {
            #region PRIVMSG_RECEIVE
            {
                string rawMessage = "@badge-info=subscriber/17;badges=subscriber/12,premium/1;client-nonce=9e99ee99999c9e9ee9ee9e99e9e99999;color=;display-name=�W�����h�D;emotes=;first-msg=0;flags=;id=999eee99-eee9-99e9-99ee-e9eee9e9e9e9;mod=0;room-id=12345678;subscriber=1;tmi-sent-ts=1653573024309;turbo=0;user-id=987654321;user-type= :johndoe!johndoe@johndoe.tmi.twitch.tv PRIVMSG #fps_paul :�G�C���ǂ����I";
                MessageInfo messageInfo = MessageParser.CreateMessageInfo(rawMessage);
                Assert.AreEqual("@badge-info=subscriber/17;badges=subscriber/12,premium/1;client-nonce=9e99ee99999c9e9ee9ee9e99e9e99999;color=;display-name=�W�����h�D;emotes=;first-msg=0;flags=;id=999eee99-eee9-99e9-99ee-e9eee9e9e9e9;mod=0;room-id=12345678;subscriber=1;tmi-sent-ts=1653573024309;turbo=0;user-id=987654321;user-type=",
                    messageInfo.TagsRaw);
                Assert.AreEqual(":johndoe!johndoe@johndoe.tmi.twitch.tv",
                    messageInfo.PrefixRaw);
                Assert.AreEqual("PRIVMSG", messageInfo.CommandRaw);
                Assert.AreEqual("#fps_paul :�G�C���ǂ����I", messageInfo.ParametersRaw);
                Assert.AreEqual(MessageType.PRIVMSG, messageInfo.MessageType);
            }
            #endregion

            #region PING
            {
                string rawMessage = "PING :tmi.twitch.tv";
                MessageInfo messageInfo = MessageParser.CreateMessageInfo(rawMessage);
                Assert.AreEqual("", messageInfo.TagsRaw);
                Assert.AreEqual("", messageInfo.PrefixRaw);
                Assert.AreEqual("PING", messageInfo.CommandRaw);
                Assert.AreEqual(":tmi.twitch.tv", messageInfo.ParametersRaw);
            }
            #endregion

            #region CAP
            {
                string rawMessage = ":tmi.twitch.tv CAP * ACK :twitch.tv/commands twitch.tv/tags";
                MessageInfo messageInfo = MessageParser.CreateMessageInfo(rawMessage);
                Assert.AreEqual("", messageInfo.TagsRaw);
                Assert.AreEqual(":tmi.twitch.tv",messageInfo.PrefixRaw);
                Assert.AreEqual("CAP", messageInfo.CommandRaw);
                Assert.AreEqual("* ACK :twitch.tv/commands twitch.tv/tags", messageInfo.ParametersRaw);
                Assert.AreEqual(MessageType.NOTSUPPORTED, messageInfo.MessageType);
            }
            #endregion
        }

        [TestMethod]
        public void MessageInfo����eParameter�̒l���擾����()
        {
            #region PRIVMSG
            {
                string rawMessage = "@badge-info=subscriber/17;badges=subscriber/12,premium/1;client-nonce=9e99ee99999c9e9ee9ee9e99e9e99999;color=;display-name=�W�����h�D;emotes=;first-msg=0;flags=;id=999eee99-eee9-99e9-99ee-e9eee9e9e9e9;mod=0;room-id=12345678;subscriber=1;tmi-sent-ts=1653573024309;turbo=0;user-id=987654321;user-type= :johndoe!johndoe@johndoe.tmi.twitch.tv PRIVMSG #fps_paul :�G�C���ǂ����I";
                MessageInfo messageInfo = MessageParser.CreateMessageInfo(rawMessage);
                Assert.AreEqual("fps_paul", messageInfo.Parameters["channel"]);
                Assert.AreEqual("�G�C���ǂ����I", messageInfo.Parameters["chat-message"]);
            }
            #endregion

            #region PING
            {
                string rawMessage = "PING :tmi.twitch.tv";
                MessageInfo messageInfo = MessageParser.CreateMessageInfo(rawMessage);
                Assert.AreEqual("tmi.twitch.tv", messageInfo.Parameters["text"]);

            }
            #endregion
        }

        [TestMethod]
        public void MessageInfo����eTags�̒l���擾����()
        {
            /*
             * PRIVMSG�ɂ���
             * �Q�l�Fhttps://dev.twitch.tv/docs/irc/send-receive-messages
             * 
             * Twitch IRC Tags�ɂ���
             * �Q�l�Fhttps://dev.twitch.tv/docs/irc/tags
             * Tags are in the form: @tag-name-1=<tag-value-1>;tag-name-2=<tag-value-2>;....
             */

            #region PRIVMSG
            string rawMessage = "@badge-info=subscriber/17;badges=subscriber/12,premium/1;client-nonce=9e99ee99999c9e9ee9ee9e99e9e99999;color=;display-name=�W�����h�D;emotes=;first-msg=0;flags=;id=999eee99-eee9-99e9-99ee-e9eee9e9e9e9;mod=0;room-id=12345678;subscriber=1;tmi-sent-ts=1653573024309;turbo=0;user-id=987654321;user-type= :johndoe!johndoe@johndoe.tmi.twitch.tv PRIVMSG #fps_paul :�G�C���ǂ����I";
            MessageInfo messageInfo = MessageParser.CreateMessageInfo(rawMessage);
            Assert.AreEqual("subscriber/17", messageInfo.Tags["badge-info"]);
            Assert.AreEqual("subscriber/12,premium/1", messageInfo.Tags["badges"]);
            Assert.AreEqual("9e99ee99999c9e9ee9ee9e99e9e99999", messageInfo.Tags["client-nonce"]);
            Assert.AreEqual("", messageInfo.Tags["color"]);
            Assert.AreEqual("�W�����h�D", messageInfo.Tags["display-name"]);
            Assert.AreEqual("", messageInfo.Tags["emotes"]);
            Assert.AreEqual("0", messageInfo.Tags["first-msg"]);
            Assert.AreEqual("", messageInfo.Tags["flags"]);
            Assert.AreEqual("999eee99-eee9-99e9-99ee-e9eee9e9e9e9", messageInfo.Tags["id"]);
            Assert.AreEqual("0", messageInfo.Tags["mod"]);
            Assert.AreEqual("12345678", messageInfo.Tags["room-id"]);
            Assert.AreEqual("1", messageInfo.Tags["subscriber"]);
            Assert.AreEqual("1653573024309", messageInfo.Tags["tmi-sent-ts"]);
            Assert.AreEqual("0", messageInfo.Tags["turbo"]);
            Assert.AreEqual("987654321", messageInfo.Tags["user-id"]);
            Assert.AreEqual("", messageInfo.Tags["user-type"]);
            #endregion
        }
    }
}