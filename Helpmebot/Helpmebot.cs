﻿// /****************************************************************************
//  *   This file is part of Helpmebot.                                        *
//  *                                                                          *
//  *   Helpmebot is free software: you can redistribute it and/or modify      *
//  *   it under the terms of the GNU General Public License as published by   *
//  *   the Free Software Foundation, either version 3 of the License, or      *
//  *   (at your option) any later version.                                    *
//  *                                                                          *
//  *   Helpmebot is distributed in the hope that it will be useful,           *
//  *   but WITHOUT ANY WARRANTY; without even the implied warranty of         *
//  *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
//  *   GNU General Public License for more details.                           *
//  *                                                                          *
//  *   You should have received a copy of the GNU General Public License      *
//  *   along with Helpmebot.  If not, see <http://www.gnu.org/licenses/>.     *
//  ****************************************************************************/
#region Usings

using System;
using System.Collections;
using System.Reflection;
using helpmebot6.AI;
using helpmebot6.Commands;
using helpmebot6.Monitoring;
using helpmebot6.Monitoring.PageWatcher;
using helpmebot6.NewYear;
using helpmebot6.Threading;
using helpmebot6.UdpListener;

#endregion

namespace helpmebot6
{
    /// <summary>
    /// Helpmebot main class
    /// </summary>
    public class Helpmebot6
    {
        public static IAL irc;
        private static DAL _dbal;
        private static Configuration _config;
        private static UDPListener udp;
        private static MonitorService nagMon;

        private static string _trigger;

        public static string debugChannel;
        public static string mainChannel;

        private static uint _ircNetwork;

        public static readonly DateTime StartupTime = DateTime.Now;

        public static bool pagewatcherEnabled = true;
        public static bool enableTwitter = true;

// ReSharper disable InconsistentNaming
        private static void Main(string[] args)
// ReSharper restore InconsistentNaming
        {
            // startup arguments
            int configFileArg = GlobalFunctions.prefixIsInArray("--configfile", args);
            string configFile = ".hmbot";
            if (configFileArg != -1)
            {
                configFile = args[configFileArg].Substring(args[configFileArg].IndexOf('='));
            }

            if (GlobalFunctions.prefixIsInArray("--logdal", args) != -1)
                Logger.instance().logDAL = true;
            if (GlobalFunctions.prefixIsInArray("--logdallock", args) != -1)
                Logger.instance().logDalLock = true;
            if (GlobalFunctions.prefixIsInArray("--logirc", args) != -1)
                Logger.instance().logIrc = true;

            if (GlobalFunctions.prefixIsInArray("--disablepagewatcher", args) != -1)
                pagewatcherEnabled = false;
            if (GlobalFunctions.prefixIsInArray("--disabletwitter", args) != -1)
                enableTwitter = false;


            initialiseBot(configFile);
        }

        private static void initialiseBot(string configFile)
        {
            string username;
            string password;
            string schema;
            uint port = 0;
            string server = username = password = schema = "";

            Configuration.readHmbotConfigFile(configFile, ref server, ref username, ref password, ref port, ref schema);

            _dbal = DAL.singleton(server, port, username, password, schema);

            if (!_dbal.connect())
            {
                // can't connect to database, DIE
                return;
            }

            Configuration.singleton();

            debugChannel = Configuration.singleton()["channelDebug"];

            _ircNetwork = uint.Parse(Configuration.singleton()["ircNetwork"]);

            _trigger = Configuration.singleton()["commandTrigger"];

            irc = new IAL(_ircNetwork);

            new IrcProxy(irc, int.Parse(Configuration.singleton()["proxyPort"]), Configuration.singleton()["proxyPassword"]);

            PageWatcherController.instance();

            setupEvents();

            TimeMonitor.instance();

            if (!irc.connect())
            {
                // if can't connect to irc, die
                return;
            }

            new MonitorService(62167, "Helpmebot v6 (Nagios Monitor service)");

            new UDPListener(4357);

            string[] twparms = {server, schema, irc.ircServer};
            try
            {
                new Twitter().updateStatus(new Message().get("tweetStartup", twparms));
            }
            catch(Twitterizer.TwitterizerException ex)
            {
                GlobalFunctions.errorLog( ex );
            }

            // ciavc hook
            irc.privmsgEvent+=AccAi.checkCiaVcCommits;
        }


        private static void setupEvents()
        {
            irc.connectionRegistrationSucceededEvent += joinChannels;

            irc.joinEvent += welcomeNewbieOnJoinEvent;

            irc.privmsgEvent += receivedMessage;

            irc.inviteEvent += irc_InviteEvent;

            irc.threadFatalError += irc_ThreadFatalError;

            PageWatcherController.instance().pageWatcherNotificationEvent += pageWatcherNotificationEvent;
        }

        private static void irc_ThreadFatalError(object sender, EventArgs e)
        {
            stop();
        }

        private static void pageWatcherNotificationEvent(PageWatcherController.RcPageChange rcItem)
        {
            string[] messageParams = {
                                         rcItem.title, rcItem.user, rcItem.comment, rcItem.diffUrl, rcItem.byteDiff,
                                         rcItem.flags
                                     };
            string message = new Message().get("pageWatcherEventNotification", messageParams);

            DAL.Select q = new DAL.Select("channel_name");
            q.addJoin("channel", DAL.Select.JoinTypes.Inner,
                      new DAL.WhereConds(false, "pwc_channel", "=", false, "channel_id"));
            q.addJoin("watchedpages", DAL.Select.JoinTypes.Inner,
                      new DAL.WhereConds(false, "pw_id", "=", false, "pwc_pagewatcher"));
            q.addWhere(new DAL.WhereConds("pw_title", rcItem.title));
            q.setFrom("pagewatcherchannels");

            ArrayList channels = DAL.singleton().executeSelect(q);

            foreach (object[] item in channels)
            {
                string channel = (string) item[0];
                if (Configuration.singleton()["silence",channel] == "false")
                    irc.ircPrivmsg(channel, message);
            }
        }

        private static void irc_InviteEvent(User source, string nickname, string channel)
        {
            new Join().run(source, nickname, new[] {channel});
        }

        private static void welcomeNewbieOnJoinEvent(User source, string channel)
        {
            NewbieWelcomer.instance().execute(source, channel);
        }

        private static void receivedMessage(User source, string destination, string message)
        {
            CommandParser cmd = new CommandParser();
            try
            {
                bool overrideSilence = cmd.overrideBotSilence;
                if (CommandParser.isRecognisedMessage(ref message, ref overrideSilence))
                {
                    cmd.overrideBotSilence = overrideSilence;
                    string[] messageWords = message.Split(' ');
                    string command = messageWords[0];
                    string[] commandArgs = string.Join(" ", messageWords, 1, messageWords.Length - 1).Split(' ');

                    cmd.handleCommand(source, destination, command, commandArgs);
                }
                string aiResponse = Intelligence.singleton().respond(message);
                if (Configuration.singleton()["silence",destination] == "false" &&
                    aiResponse != string.Empty)
                {
                    string[] aiParameters = {source.nickname};
                    irc.ircPrivmsg(destination, new Message().get(aiResponse, aiParameters));
                }
            }
            catch (Exception ex)
            {
                GlobalFunctions.errorLog(ex);
            }
        }

        private static void joinChannels()
        {
            irc.ircJoin(debugChannel);

            DAL.Select q = new DAL.Select("channel_name");
            q.setFrom("channel");
            q.addWhere(new DAL.WhereConds("channel_enabled", 1));
            q.addWhere(new DAL.WhereConds("channel_network", _ircNetwork.ToString()));
            foreach (object[] item in _dbal.executeSelect(q))
            {
                irc.ircJoin((string) (item)[0]);
            }
        }

        public static void stop()
        {
            ThreadList.instance().stop();
        }

        public static string trigger
        {
            get
            {
                return _trigger;
            }
            set
            {
                _trigger = value;
            }
        }
    }
}