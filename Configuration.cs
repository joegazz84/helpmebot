/****************************************************************************
 *   This file is part of Helpmebot.                                        *
 *                                                                          *
 *   Helpmebot is free software: you can redistribute it and/or modify      *
 *   it under the terms of the GNU General Public License as published by   *
 *   the Free Software Foundation, either version 3 of the License, or      *
 *   (at your option) any later version.                                    *
 *                                                                          *
 *   Helpmebot is distributed in the hope that it will be useful,           *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *   GNU General Public License for more details.                           *
 *                                                                          *
 *   You should have received a copy of the GNU General Public License      *
 *   along with Helpmebot.  If not, see <http://www.gnu.org/licenses/>.     *
 ****************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace helpmebot6
{
    public class Configuration
    {
        public static void readHmbotConfigFile( string filename, 
                ref string mySqlServerHostname, ref string mySqlUsername, 
                ref string mySqlPassword, ref uint mySqlServerPort, 
                ref string mySqlSchema )
        {
            StreamReader settingsreader = new StreamReader( filename );
            mySqlServerHostname = settingsreader.ReadLine( );
            mySqlServerPort = uint.Parse( settingsreader.ReadLine( ) );
            mySqlUsername = settingsreader.ReadLine( );
            mySqlPassword = settingsreader.ReadLine( );
            mySqlSchema = settingsreader.ReadLine( );
            settingsreader.Close( );
        }
    }
}
