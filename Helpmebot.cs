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
using System.Collections;
using helpmebot6.Threading;
namespace helpmebot6
{
    public class Helpmebot6
    {
       public static IAL irc ;

       static string Trigger;

       public static string debugChannel;
       public static string mainChannel;


       public static readonly DateTime startupTime = DateTime.Now;


       static void Main( string[ ] args )
       {
           // startup arguments
           int configFileArg = GlobalFunctions.prefixIsInArray( "--configfile", args );
           string configFile = ".hmbot";
           if ( configFileArg!=-1 )
           {
               configFile = args[ configFileArg ].Substring( args[ configFileArg ].IndexOf( '=' ) );
           }

           if( GlobalFunctions.prefixIsInArray( "--logirc" , args ) != -1 )
               Logger.Instance( ).LogIRC = true;

           InitialiseBot( configFile );
       }

       private static void InitialiseBot( string configFile )
       {
           string server, username, password, schema;
           uint port = 0;
           server = username = password = schema = "";

           Configuration.readHmbotConfigFile( configFile, ref server, ref username, ref password, ref port, ref schema );





           Trigger ="!";

           irc = new IAL( server, port, username, password, "hmb", schema );


           SetupEvents( );

           if( !irc.Connect( ) )
           { // if can't connect to irc, die
               return;
           }
       }



       static void SetupEvents( )
       {
           irc.ConnectionRegistrationSucceededEvent += new IAL.ConnectionRegistrationEventHandler( JoinChannels );

           irc.PrivmsgEvent += new IAL.PrivmsgEventHandler( ReceivedMessage );

           irc.ThreadFatalError += new EventHandler( irc_ThreadFatalError );
       }

       static void irc_ThreadFatalError( object sender, EventArgs e )
       {
           Stop( );
       }


       static void ReceivedMessage( User source , string destination , string message )
       {
           CommandParser cmd = new CommandParser( );
           try
           {
               bool overrideSilence = cmd.overrideBotSilence;
               if( isRecognisedMessage( ref message , ref overrideSilence ) )
               {
                   cmd.overrideBotSilence = overrideSilence;
                   string[ ] messageWords = message.Split( ' ' );
                   string command = messageWords[ 0 ];
                   string[ ] commandArgs = string.Join( " " , messageWords , 1 , messageWords.Length - 1 ).Split( ' ' );

                   cmd.handleCommand( source , destination , command , commandArgs );


               }
           }
           catch( Exception ex )
           {
               GlobalFunctions.ErrorLog( ex  );
           }


       }

       static void JoinChannels( )
        {
            debugChannel = "##helpmebot";
            irc.IrcJoin( debugChannel );

            irc.IrcJoin( "#wikipedia-en-help" );
        }

        /// <summary>
        /// Tests against recognised message formats
        /// </summary>
        /// <param name="message">the message recieved</param>
        /// <param name="overrideSilence">ref: whether this message format overrides any imposed silence</param>
        /// <returns>true if the message is in a recognised format</returns>
        /// <remarks>Allowed formats:
        /// !command
        /// !helpmebot command
        /// Helpmebot: command
        /// Helpmebot command
        /// Helpmebot, command
        /// Helpmebot> command
        /// </remarks>
       static bool isRecognisedMessage( ref string message , ref bool overrideSilence )
       {
           string[ ] words = message.Split( ' ' );

           if( words[ 0 ].StartsWith( Trigger ) )
           {

               /// !

               if( message.Length == Trigger.Length )
                   return false;

               /// !command
               /// !helpmebot command


               if( words[ 0 ].ToLower() == ( Trigger + irc.IrcNickname.ToLower()) )
               {
                   overrideSilence = true;
                   message = string.Join( " " , words , 1 , words.Length - 1 );
                   return true;
               }
               else
               {
                   message = message.Substring( 1 );
                   overrideSilence = false;
                   return true;
               }
           }
           else
           {
               if( words[ 0 ].ToLower() == irc.IrcNickname.ToLower() )/// Helpmebot command
               {
                   message = string.Join( " " , words , 1 , words.Length - 1 );
                   overrideSilence = true;
                   return true;
               }
               else if( words[ 0 ].ToLower() == ( irc.IrcNickname.ToLower() + ":" ) ) /// Helpmebot: command
               {
                   message = string.Join( " " , words , 1 , words.Length - 1 );
                   overrideSilence = true;
                   return true;
               }
               else if( words[ 0 ].ToLower() == ( irc.IrcNickname.ToLower() + ">" ) ) /// Helpmebot> command
               {
                   message = string.Join( " " , words , 1 , words.Length - 1 );
                   overrideSilence = true;
                   return true;
               }
               else if(words[ 0 ].ToLower() == (irc.IrcNickname.ToLower() + ",")) /// Helpmebot, command
               {
                   message = string.Join( " " , words , 1 , words.Length - 1 );
                   overrideSilence = true;
                   return true;
               }

           }
           return false;
       }

       static public void Stop( )
       {
           ThreadList.instance( ).stop( );
       }
    }
}
