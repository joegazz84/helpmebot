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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using helpmebot6.Threading;
using MySql.Data.MySqlClient;

#endregion

namespace helpmebot6.NewYear
{
    /// <summary>
    /// Time monitor class for new year greetings
    /// </summary>
    internal class TimeMonitor : IThreadedSystem
    {
        public static TimeMonitor instance()
        {
            return _instance ?? ( _instance = new TimeMonitor( ) );
        }

        private static TimeMonitor _instance;
        private readonly Thread _monitorThread;

        private readonly string _targetDate;

        private readonly Dictionary<DateTime, string> _timezoneList;

        protected TimeMonitor()
        {
            this._targetDate = Configuration.singleton()["newYearDateMonitoringTarget"];

            this._monitorThread = new Thread(monitorThreadMethod);

            this._timezoneList = new Dictionary<DateTime, string>();

            DAL.Select q = new DAL.Select(
                "tz_places",
                "ADDDATE(ADDDATE(\"" + MySqlHelper.EscapeString(this._targetDate) +
                "\", INTERVAL -tz_offset_hours HOUR), INTERVAL -tz_offset_minutes MINUTE)"
                );
            q.setFrom("timezones");
            q.escapeSelects(false);
            q.addOrder(
                new DAL.Select.Order(
                    "ADDDATE(ADDDATE(\"" + MySqlHelper.EscapeString(this._targetDate) +
                    "\", INTERVAL -tz_offset_hours HOUR), INTERVAL -tz_offset_minutes MINUTE)", false, false));

            ArrayList al = DAL.singleton().executeSelect(q);

            foreach (object[] row in al)
            {
                this._timezoneList.Add((DateTime.Parse(new String(Encoding.UTF8.GetChars(((byte[]) (row)[1]))))),
                                 (string) (row[0]));
            }

            this.registerInstance();
            this._monitorThread.Start();
        }

        private void monitorThreadMethod()
        {
            try
            {
                while (this._timezoneList.Count > 0)
                {
                    string places;
                    if (this._timezoneList.TryGetValue(DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")),
                                                 out places))
                    {
                        sendNewYearMessage(places);
                        Thread.Sleep(1000);
                    }
                    Thread.Sleep(500);
                }
            }
            catch (ThreadAbortException)
            {
                EventHandler temp = this.threadFatalError;
                if (temp != null)
                {
                    temp(this, new EventArgs());
                }
            }
        }

        private static void sendNewYearMessage(string places)
        {
            DAL.Select q = new DAL.Select("channel_name");
            q.setFrom("channel");
            q.addWhere(new DAL.WhereConds("channel_enabled", "1"));


            foreach (object[] res in DAL.singleton().executeSelect(q))
            {
                string channel = res[ 0 ].ToString( );
                if ( Configuration.singleton()["newYearDateAlerting",channel] != "true" )
                    continue;
                string[ ] args = { places };
                string message = new Message().get( "newYearMessage", args );
                Helpmebot6.irc.ircPrivmsg( channel, message );
                new Twitter().updateStatus( message );
            }
        }

        #region IThreadedSystem Members

        public void stop()
        {
            this._monitorThread.Abort();
        }

        public void registerInstance()
        {
            ThreadList.instance().register(this);
        }

        public string[] getThreadStatus()
        {
            string[] statuses = {this._targetDate + " " + this._monitorThread.ThreadState};
            return statuses;
        }

        public event EventHandler threadFatalError;

        #endregion
    }
}