// /****************************************************************************
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
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

#endregion

namespace helpmebot6.Monitoring
{
    /// <summary>
    /// Newbie welcomer subsystem
    /// </summary>
    internal class NewbieWelcomer
    {
        private static NewbieWelcomer _instance;

        protected NewbieWelcomer()
        {
         
        }

        public static NewbieWelcomer instance()
        {
            return _instance ?? ( _instance = new NewbieWelcomer( ) );
        }


        /// <summary>
        /// Executes the newbie.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="channel">The channel.</param>
        public void execute(User source, string channel)
        {
            if (Configuration.singleton()["silence",channel] == "false" &&
                Configuration.singleton()["welcomeNewbie",channel] == "true")
            {

                bool match = false;

                // check against host list in welcomeusers table where exception = 0
                // if matches, set match=true
                
                // if match = true, check host against welcomeusers where exception = 1
                // if matches, set match = false

                if (match)
                {
                    string[] cmdArgs = {source.nickname, channel};
                    Helpmebot6.irc.ircPrivmsg(channel, new Message().get("newbieWelcome" + channel.Replace('#',':'), cmdArgs));
                }
            }
        }

        /// <summary>
        /// Adds a host to the list of detected newbie hosts.
        /// </summary>
        /// <param name="host">The host.</param>
        [Obsolete]
        public void addHost(string host)
        {
            throw new NotImplementedException();
        }

        public void addHost(User nuh, string channel, bool exception)
        {
            throw new NotImplementedException();
        }

        [Obsolete]
        public void delHost(string host)
        {
            throw new NotImplementedException();
        }

        public void delHost(User nuh, string channel, bool exception)
        {
            throw new NotImplementedException();
        }

        [Obsolete]
        public string[] getHosts()
        {
            throw new NotImplementedException();
        }
    }
}