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

using System.Reflection;

#endregion

namespace helpmebot6.Commands
{
    /// <summary>
    /// Enables or disables automatic parsing of wikilinks
    /// </summary>
    internal class Autolink : GenericCommand
    {
        /// <summary>
        /// Actual command logic
        /// </summary>
        /// <param name="source">The user who triggered the command.</param>
        /// <param name="channel">The channel the command was triggered in.</param>
        /// <param name="args">The arguments to the command.</param>
        /// <returns></returns>
        protected override CommandResponseHandler execute(User source, string channel, string[] args)
        {
            Logger.instance().addToLog(
                "Method:" + MethodBase.GetCurrentMethod().DeclaringType.Name + MethodBase.GetCurrentMethod().Name,
                Logger.LogTypes.DNWB);

            bool global = false;


            if (args.Length > 0)
            {
                if (args[0].ToLower() == "@global")
                {
                    global = true;
                    GlobalFunctions.popFromFront(ref args);
                }
            }

            bool oldValue = bool.Parse( !global ? Configuration.singleton().retrieveLocalStringOption("autoLink", channel) : Configuration.singleton().retrieveGlobalStringOption("autoLink") );

            if (args.Length > 0)
            {
                string newValue = "global";
                switch (args[0].ToLower())
                {
                    case "enable":
                        newValue = "true";
                        break;
                    case "disable":
                        newValue = "false";
                        break;
                    case "global":
                        newValue = "global";
                        break;
                }
                if (newValue == oldValue.ToString().ToLower())
                {
                    return new CommandResponseHandler(Configuration.singleton().getMessage("no-change"),
                                                      CommandResponseDestination.PrivateMessage);
                }
                if (newValue == "global")
                {
                    Configuration.singleton().deleteLocalOption("autoLink", channel);
                    return new CommandResponseHandler(Configuration.singleton().getMessage("defaultConfig"),
                                                      CommandResponseDestination.PrivateMessage);
                }
                if (!global)
                    Configuration.singleton().oldSetLocalOption("autoLink", channel, newValue);
                else
                    Configuration.singleton().oldSetGlobalOption("autoLink", newValue);
                return new CommandResponseHandler(Configuration.singleton().getMessage("done"),
                                                  CommandResponseDestination.PrivateMessage);
            }
            string[] mP = {"autolink", 1.ToString(), args.Length.ToString()};
            return new CommandResponseHandler(Configuration.singleton().getMessage("notEnoughParameters", mP),
                                              CommandResponseDestination.PrivateMessage);
        }
    }
}