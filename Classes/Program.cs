using ChatSharp;
using ChatSharp.Events;
using RRpain.Classes.Common;
using RRpain.Classes.Core;
using RRpain.Classes.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Timers;

namespace RRpain.Classes
{
    static class Program
    {
        public static Timer AnnounceTimer;
        public static IrcClient Client;

        public static string McBansApiUrl = "";
        private static string _nickServAuth = "";
        private static string _commandPrefix = "";

        public static bool DevMode;
        public static bool IsLocked;
      
        public static readonly Dictionary<string, CommandAttribute> Commands = new Dictionary<string, CommandAttribute>();
    
        static void Main(string[] argArray)
        {
            InitClasses();

            if (argArray.Any()) _nickServAuth = argArray[0];

            //set the command prefix to $ if debug mode
            _commandPrefix = string.IsNullOrEmpty(_nickServAuth) ? "$" : "!";
            DevMode = string.IsNullOrEmpty(_nickServAuth);

            Client = (!string.IsNullOrEmpty(_nickServAuth)) ? new IrcClient(Config.Host, new IrcUser(Config.Nickname, Config.Username)) : new IrcClient(Config.Host, new IrcUser(Config.NickTest, Config.UserTest));
            Client.NetworkError += OnNetworkError;
            Client.ConnectionComplete += OnConnectionComplete;
            Client.UserMessageRecieved += OnUserMessageReceived;
            Client.ChannelMessageRecieved += OnChannelMessageReceived;
            Client.UserJoinedChannel += OnUserJoinedChannel;

            AnnounceTimer = new Timer();
            AnnounceTimer.Elapsed += OnTimedEvent;

            if (string.IsNullOrEmpty(_nickServAuth))
            {
                Utils.Log("Warning, nick serv authentication password is empty.");
            }

            Utils.Log("Connecting to IRC...");
            Client.ConnectAsync();
            Console.ReadLine();
        }

        private static void InitClasses()
        {
            var classes =
                Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(type => type.IsClass && type.BaseType.Name == "CommandHandler");

            foreach (var cls in classes)
            {
                var attrib = cls.GetCustomAttributes(typeof(CommandAttribute), true).FirstOrDefault() as CommandAttribute;
                Commands.Add(cls.FullName, attrib);
            }
        }

        private static void OnNetworkError(object s, SocketErrorEventArgs e)
        {
            Utils.Log("Error: " + e.SocketError);
        }

        private static void OnUserJoinedChannel(object sender, ChannelUserEventArgs args)
        {
            Utils.SendNotice(String.Format(Data.MessageJoinChannel, args.User.Nick), args.User.Nick);
        }

        private static void OnUserMessageReceived(object s, PrivateMessageEventArgs e)
        {
            if (!Utils.IsDev(e.PrivateMessage.User.Nick)) return;
            if (e.PrivateMessage.Message.StartsWith(_commandPrefix + "msg "))
                Utils.SendChannel(e.PrivateMessage.Message.Substring(5));
        }

        private static void OnChannelMessageReceived(object sender, PrivateMessageEventArgs args)
        {
            var serverList = new List<string> {"RR1", "RR2", "MagicFarm", "Dire20", "Unleashed", "Pixelmon", "TPPI", "Horizons"};
            var isIngameCommand = false;
            var message = args.PrivateMessage.Message.Trim();
            var paramList = message.Split(' ');

            if (serverList.Contains(args.PrivateMessage.User.Nick))
            {
                var ingameMessage = args.PrivateMessage.Message.Split(':');
                if (ingameMessage.Any())
                {
                    try
                    {
                        if (ingameMessage[1].StartsWith(" " + _commandPrefix))
                        {
                            paramList = ingameMessage[1].Trim().Split(' ');
                            isIngameCommand = true;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (args.PrivateMessage.Message.StartsWith(_commandPrefix) || paramList[0].StartsWith(_commandPrefix))
            {
               
                    if (IsLocked & Utils.IsOp(args.PrivateMessage.User.Nick) != true & !paramList[0].Contains("player"))
                    {
                        Utils.SendNotice("RRpain is currently set to Ops Only.", args.PrivateMessage.User.Nick);
                    }
                    else
                    {
                        foreach (var type in Commands.Where(cmd => cmd.Value.Listener == paramList[0].Substring(1)).Select(cmd => Type.GetType(cmd.Key)).Where(type => type != null))
                        {
                            ((CommandHandler)Activator.CreateInstance(type)).HandleCommand(paramList, args.PrivateMessage.User, isIngameCommand);
                        }
                    }
               
            }

            //listen for www or http(s)
            if (!string.IsNullOrEmpty(_nickServAuth))
            {
                if (args.PrivateMessage.Message.Contains("http://") || args.PrivateMessage.Message.Contains("https://") || args.PrivateMessage.Message.Contains("www."))
                {
                    foreach (var item in paramList)
                    {
                        var url = "";
                        if (item.Contains("http://") || item.Contains("https://"))
                        {
                            url = item;
                        }
                        else if (item.Contains("www."))
                        {
                            url = string.Concat("http://", item);
                        }

                        if (!string.IsNullOrEmpty(url))
                        {
                            Connection.GetLinkTitle(url, title =>
                            {
                                if (!string.IsNullOrEmpty(title))
                                {
                                    Utils.SendChannel("URL TITLE: " + title);
                                }
                                else
                                {
                                    Utils.Log("Connection: Result is null");
                                }
                            }, Utils.HandleException);
                        }
                    }
                }
            }

            if (args.PrivateMessage.Message.StartsWith(_commandPrefix))
            {
                Utils.Log("<{0}> {1}", args.PrivateMessage.User.Nick, args.PrivateMessage.Message);
            }
        }

        private static void OnConnectionComplete(object s, EventArgs e)
        {
            Utils.Log("Connection complete.");
            if (!string.IsNullOrEmpty(_nickServAuth))
            {
                Utils.Log("Sending ident message to NickServ");
                Utils.SendPm(string.Format("IDENTIFY RRpain {0}", _nickServAuth), "NickServ");
            }
            else
            {
                Utils.Log("No NickServ authentication detected.");
                JoinChannel();
            }

            Client.RawMessageRecieved += (sender, args) =>
            {
                if (args.Message != Data.MessageIdentified)
                    return;
                Utils.Log("NickServ authentication was successful.");
                JoinChannel();
            };

        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (Convert.ToInt32(Announcement.AnnounceTimes) == 0)
            {
                AnnounceTimer.Enabled = false;
            }
            else
            {
                var count = Convert.ToInt32(Announcement.AnnounceTimes);
                count--;
                Announcement.AnnounceTimes = count;
                Utils.SendChannel(Announcement.AnnounceMsg.ToString());
            }
        }

        private static void JoinChannel()
        {
            Client.JoinChannel(DevMode ? Config.DevChannel : Config.Channel);
            Utils.Log("Joining channel: {0}", DevMode ? Config.DevChannel : Config.Channel);
        }
    }
}
