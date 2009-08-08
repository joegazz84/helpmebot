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
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace helpmebot6
{
    /// <summary>
    /// IRC Access Layer
    /// 
    /// Provides an interface to IRC.
    /// </summary>
    public class IAL
    {
        #region internal variables
        public static IAL singleton;

        string _myNickname;
        string _myUsername;
        string _myRealname;
        string _myPassword;

        string _ircServer;
        uint _ircPort;

        TcpClient _tcpClient;
        StreamReader _ircReader;
        StreamWriter _ircWriter;

        Queue _sendQ;

        int _floodProtectionWaitTime = 500;

        int _connectionUserModes = 0;

        string _version = "Helpmebot IRC Access Layer 1.0";

        int _messageCount = 0;

        #endregion

        #region properties


        public string ClientVersion
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }

        public bool Connected
        {
            get
            {
                if ( this._tcpClient != null )
                {
                    return this._tcpClient.Connected;
                }
                else
                {
                    return false;
                }
            }
        }

        public string IrcNickname
        {
            get
            {
                return _myNickname;
            }
            set
            {
            }
        }
        public string IrcUsername
        {
            get
            {
                return _myUsername;
            }
        }
        public string IrcRealname
        {
            get
            {
                return _myRealname;
            }
        }

        public string IrcServer
        {
            get
            {
                return _ircServer;
            }
        }
        public uint IrcPort
        {
            get
            {
                return _ircPort;
            }
        }

        public int FloodProtectionWaitTime
        {
            get
            {
                return _floodProtectionWaitTime;
            }
            set
            {
                _floodProtectionWaitTime = value;
            }
        }

        /// <summary>
        /// +4 if recieving wallops, +8 if invisible
        /// </summary>
        public int ConnectionUserModes
        {
            get
            {
                return _connectionUserModes;
            }
        }

        public int MessageCount
        {
            get
            {
                return _messageCount;
            }
        }
        #endregion

        #region constructor/destructor

        public IAL( string Nickname, string Username, string Realname, string Password, string IrcServer, uint IrcPort, bool recieveWallops, bool invisible )
        {
            _myRealname = Realname;
            _myNickname = Nickname;
            _myUsername = Username;
            _myPassword = Password;

            _ircServer = IrcServer;
            _ircPort = IrcPort;

            if ( recieveWallops )
                _connectionUserModes += 4;
            if ( invisible )
                _connectionUserModes += 8;

            initialiseEventHandlers( );
        }

        ~IAL( )
        {

        }

        #endregion

        #region Methods

        public void Connect( )
        {
            try
            {
                _tcpClient = new TcpClient( _ircServer, (int)_ircPort );

                Stream _IrcStream = _tcpClient.GetStream( );
                _ircReader = new StreamReader( _IrcStream );
                _ircWriter = new StreamWriter( _IrcStream );


                _sendQ = new Queue( 100 );

                ThreadStart _ircReaderThreadStart = new ThreadStart( _ircReaderThreadMethod );
                _ircReaderThread = new Thread( _ircReaderThreadStart );
                _ircReaderThread.Start( );

                ThreadStart _ircWriterThreadStart = new ThreadStart( _ircWriterThreadMethod );
                _ircWriterThread = new Thread( _ircWriterThreadStart );
                _ircWriterThread.Start( );

                ConnectionRegistrationRequiredEvent( );
            }
            catch ( SocketException ex )
            {
                GlobalFunctions.ErrorLog( ex , System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        void _sendLine( string line )
        {
            if ( this.Connected )
            {
                line = line.Replace( "\n", " " );
                line = line.Replace( "\r", " " );
                _sendQ.Enqueue( line.Trim() );
                _messageCount++;
            }
        }

        void _sendPass( string password )
        {
            _sendLine( "PASS " + password );
        }

        void _sendNick( string nickname )
        {
            _sendLine( "NICK " + nickname );
        }

        /// <summary>
        /// Sends the USER command as part of connection registration
        /// </summary>
        /// <param name="username">The client's username</param>
        /// <param name="mode">The connection user modes: bitmask, bit 3 (8) is invisible, bit 2 (4) recieves wallops</param>
        /// <param name="realname">The client's real name</param>
        void _sendUser( string username, int mode, string realname )
        {
            _sendLine( "USER " + username + " " + /*mode.ToString()*/ "*" + " * :" + realname );
        }

        void registerConnection( )
        {
            if ( _myPassword != null )
                _sendPass( _myPassword );
            _sendUser( _myUsername, _connectionUserModes, _myRealname );
            _sendNick( _myNickname );
        }

        void assumeTakenNickname( )
        {
            _sendNick( _myNickname + "_" );
            this.IrcPrivmsg( "NickServ", "GHOST " + _myNickname );
            _sendNick( _myNickname );
        }

        public void IrcPong( string datapacket )
        {
            _sendLine( "PONG " + datapacket );
        }

        public void IrcPing( string datapacket )
        {
            _sendLine( "PING " + datapacket );
        }

        /// <summary>
        /// Sends a private message
        /// </summary>
        /// <param name="Destination">The destination of the private message.</param>
        /// <param name="Message">The message text to be sent</param>
        public void IrcPrivmsg( string Destination, string Message )
        {
            if ( Message.Length > 400 )
            {
                _sendLine( "PRIVMSG " + Destination + " :" + Message.Substring( 0, 400 ) + "..." );
                IrcPrivmsg( Destination, "..." + Message.Substring( 400 ) );
            }
            else
            {
                _sendLine( "PRIVMSG " + Destination + " :" + Message );
            }
        }

        public void IrcQuit( string message )
        {
            _sendLine( "QUIT :" + message );
        }
        public void IrcQuit( )
        {
            _sendLine( "QUIT" );
        }

        public void IrcJoin( string channel )
        {
            _sendLine( "JOIN " + channel );
        }
        public void IrcJoin( string[ ] channels )
        {
            foreach ( string channel in channels )
            {
                this.IrcJoin( channel );
            }
        }

        public void IrcMode( string channel, string modeflags, string param )
        {
            _sendLine( "MODE " + channel + " " + modeflags + " " + param );
        }
        public void IrcMode( string channel, string flags )
        {
            this.IrcMode( channel, flags, "" );
        }

        public void IrcPart( string channel, string message )
        {
            _sendLine( "PART " + channel + " " + message );
        }
        public void IrcPart( string channel )
        {
            IrcPart( channel, "" );
        }
        public void PartAllChannels( )
        {
            IrcJoin( "0" );
        }

        public void IrcNames( string channel )
        {
            _sendLine( "NAMES " + channel );
        }
        public void IrcNames( )
        {
            _sendLine( "NAMES" );
        }

        public void IrcList( )
        {
            _sendLine( "LIST" );
        }
        public void IrcList( string channels )
        {
            _sendLine( "LIST " + channels );
        }

        public void IrcInvite( string nickname, string channel )
        {
            _sendLine( "INVITE " + nickname + " " + channel );
        }

        public void IrcKick( string channel, string user )
        {
            _sendLine( "KICK " + channel + " " + user );
        }
        public void IrcKick( string channel, string user, string reason )
        {
            _sendLine( "KICK" + channel + " " + user + " :" + reason );
        }

        public void CtcpReply( string destination, string command, string parameters )
        {
            ASCIIEncoding asc = new ASCIIEncoding( );
            byte[ ] ctcp = { Convert.ToByte( 1 ) };
            IrcNotice( destination, asc.GetString( ctcp ) + command.ToUpper( ) + " " + parameters + asc.GetString( ctcp ) );
        }

        public void CtcpRequest( string destination, string command )
        {
            ASCIIEncoding asc = new ASCIIEncoding( );
            byte[ ] ctcp = { Convert.ToByte( 1 ) };
            IrcPrivmsg( destination, asc.GetString( ctcp ) + command.ToUpper( ) + asc.GetString( ctcp ) );
        }

        public void IrcNotice( string destination, string message )
        {
            _sendLine( "NOTICE " + destination + " :" + message );
        }

        //TODO: Expand for network staff use
        public void IrcMotd( )
        {
            _sendLine( "MOTD" );
        }
        //TODO: Expand for network staff use
        public void IrcLusers( )
        {
            _sendLine( "LUSERS" );
        }

        //TODO: Expand for network staff use
        public void IrcVersion( )
        {
            _sendLine( "VERSION" );
        }

        public void IrcStats( string query )
        {
            _sendLine( "STATS " + query );
        }

        public void IrcLinks( string mask )
        {
            _sendLine( "LINKS " + mask );
        }

        public void IrcTime( )
        {
            _sendLine( "TIME" );
        }

        public void IrcAdmin( )
        {
            _sendLine( "ADMIN" );
        }

        public void IrcInfo( )
        {
            _sendLine( "INFO" );
        }

        public void IrcWho( string mask )
        {
            _sendLine( "WHO " + mask );
        }

        public void IrcWhois( string mask )
        {
            _sendLine( "WHOIS " + mask );
        }
        public void IrcWhowas( string mask )
        {
            _sendLine( "WHOWAS " + mask );
        }

        public void IrcKill( string nickname, string comment )
        {
            _sendLine( "KILL " + nickname + " :" + comment );
        }

        public void IrcAway( )
        {
            _sendLine( "AWAY" );
        }

        public void IrcAway( string message )
        {
            _sendLine( "AWAY :" + message );
        }

        public void IrcIson( string nicklist )
        {
            _sendLine( "ISON " + nicklist );
        }
        #endregion

        #region Threads
        Thread _ircReaderThread;
        Thread _ircWriterThread;

        void _ircReaderThreadMethod( )
        {
            bool _ThreadIsAlive = true;
            do
            {
                try
                {
                    string line = _ircReader.ReadLine( );
                    if ( line == null )
                    {
                        // noop
                    }
                    else
                    {
                        if ( this.DataRecievedEvent != null )
                            DataRecievedEvent( line );
                    }
                }
                catch ( ThreadAbortException ex )
                {
                    _ThreadIsAlive = false;
                    GlobalFunctions.ErrorLog( ex , System.Reflection.MethodInfo.GetCurrentMethod());
                }
                catch ( IOException ex )
                {
                    _ThreadIsAlive = false;
                    GlobalFunctions.ErrorLog( ex , System.Reflection.MethodInfo.GetCurrentMethod());
                }
                catch ( Exception ex )
                {
                    GlobalFunctions.ErrorLog( ex , System.Reflection.MethodInfo.GetCurrentMethod());
                }
            }
            while ( _ThreadIsAlive );

            Console.WriteLine( "*** Reader thread died." );
        }

        void _ircWriterThreadMethod( )
        {
            bool _ThreadIsAlive = true;
            do
            {
                try
                {
                    if ( _sendQ.Count > 0 )
                    {
                        string line = (string)_sendQ.Dequeue( );
                        Console.WriteLine( "<" + line );
                        _ircWriter.WriteLine( line );
                        _ircWriter.Flush( );
                        Thread.Sleep( this.FloodProtectionWaitTime );
                    }
                    else
                    {
                        // wait a short while before rechecking
                        Thread.Sleep( 100 );
                    }
                }
                catch ( ThreadAbortException ex )
                {
                    _ThreadIsAlive = false;
                    GlobalFunctions.ErrorLog( ex , System.Reflection.MethodInfo.GetCurrentMethod());
                    _sendQ.Clear( );
                }
                catch ( IOException ex )
                {
                    _ThreadIsAlive = false;
                    GlobalFunctions.ErrorLog( ex , System.Reflection.MethodInfo.GetCurrentMethod());
                }
                catch ( Exception ex )
                {
                    GlobalFunctions.ErrorLog( ex , System.Reflection.MethodInfo.GetCurrentMethod());
                }
            }
            while ( _ThreadIsAlive );
        }
        #endregion

        #region events
        public delegate void DataRecievedEventHandler( string data );
        public event DataRecievedEventHandler DataRecievedEvent;

        public delegate void ConnectionRegistrationEventHandler( );
        event ConnectionRegistrationEventHandler ConnectionRegistrationRequiredEvent;
        public event ConnectionRegistrationEventHandler ConnectionRegistrationSucceededEvent;

        public delegate void PingEventHandler( string datapacket );
        public event PingEventHandler PingEvent;

        public delegate void NicknameChangeEventHandler( string oldnick, string newnick );
        public event NicknameChangeEventHandler NicknameChangeEvent;

        public delegate void ModeChangeEventHandler( User source, string subject, string flagchanges, string parameter );
        public event ModeChangeEventHandler ModeChangeEvent;

        public delegate void QuitEventHandler( User source, string message );
        public event QuitEventHandler QuitEvent;

        public delegate void JoinEventHandler( User source, string channel );
        public event JoinEventHandler JoinEvent;

        public delegate void PartEventHandler( User source, string channel, string message );
        public event PartEventHandler PartEvent;

        public delegate void TopicEventHandler( User source, string channel, string topic );
        public event TopicEventHandler TopicEvent;

        public delegate void InviteEventHandler( User source, string nickname, string channel );
        public event InviteEventHandler InviteEvent;

        public delegate void KickEventHandler( User source, string channel, string nick, string message );
        public event KickEventHandler KickEvent;

        public delegate void PrivmsgEventHandler( User source, string destination, string message );
        public event PrivmsgEventHandler PrivmsgEvent;
        public event PrivmsgEventHandler CtcpEvent;
        public event PrivmsgEventHandler NoticeEvent;

        public delegate void IrcEventHandler( );
        public event IrcEventHandler Err_NicknameInUseEvent;



        #endregion

        private void initialiseEventHandlers( )
        {
            this.DataRecievedEvent += new DataRecievedEventHandler( IAL_DataRecievedEvent );
            this.ConnectionRegistrationRequiredEvent += new ConnectionRegistrationEventHandler( registerConnection );
            this.PingEvent += new PingEventHandler( IrcPong );
            this.NicknameChangeEvent += new NicknameChangeEventHandler( IAL_NicknameChangeEvent );
            this.QuitEvent += new QuitEventHandler( IAL_QuitEvent );
            this.JoinEvent += new JoinEventHandler( IAL_JoinEvent );
            this.PartEvent += new PartEventHandler( IAL_PartEvent );
            this.TopicEvent += new TopicEventHandler( IAL_TopicEvent );
            this.ModeChangeEvent += new ModeChangeEventHandler( IAL_ModeChangeEvent );
            this.InviteEvent += new InviteEventHandler( IAL_InviteEvent );
            this.KickEvent += new KickEventHandler( IAL_KickEvent );
            this.PrivmsgEvent += new PrivmsgEventHandler( IAL_PrivmsgEvent );
            this.CtcpEvent += new PrivmsgEventHandler( IAL_CtcpEvent );
            this.NoticeEvent += new PrivmsgEventHandler( IAL_NoticeEvent );
            this.Err_NicknameInUseEvent += new IrcEventHandler( assumeTakenNickname );
        }

        #region event handlers
        void IAL_NoticeEvent( User source, string destination, string message )
        {
            Console.WriteLine( ">>>>> NOTICE EVENT FROM " + source.ToString() + " TO " + destination + " MESSAGE " + message );
        }

        void IAL_CtcpEvent( User source, string destination, string message )
        {
            Console.WriteLine( ">>>>> CTCP EVENT FROM " + source.ToString() + " TO " + destination + " MESSAGE " + message );
            switch ( message )
            {
                case "VERSION":
                    CtcpReply( source.Nickname, "VERSION", this.ClientVersion );
                    break;
                default:
                    break;
            }
        }

        void IAL_PrivmsgEvent( User source, string destination, string message )
        {
            Console.WriteLine( ">>>>> PRIVMSG EVENT FROM " + source.ToString() + " TO " + destination + " MESSAGE " + message );
        }

        void IAL_KickEvent( User source, string channel, string nick, string message )
        {
            Console.WriteLine( ">>>>> KICK FROM " + channel + " BY " + source.ToString() + " AFFECTED " + nick + " REASON " + message );
        }

        void IAL_InviteEvent( User source, string nickname, string channel )
        {
            Console.WriteLine( ">>>>> INVITE FROM " + source.ToString() + " TO " + nickname + " CHANNEL " + channel );
        }

        void IAL_ModeChangeEvent( User source, string subject, string flagchanges, string parameter )
        {
            Console.WriteLine( ">>>>> MODE CHANGE BY " + source.ToString() + " ON " + subject + " CHANGES " + flagchanges + " PARAMETER " + parameter );
        }

        void IAL_TopicEvent( User source, string channel, string topic )
        {
            Console.WriteLine( ">>>>> TOPIC CHANGED BY " + source.ToString() + " IN " + channel + " TOPIC " + topic );
        }

        void IAL_PartEvent( User source, string channel, string message )
        {
            Console.WriteLine( ">>>>> PART BY " + source.ToString() + " FROM " + channel + " MESSAGE " + message );
        }

        void IAL_JoinEvent( User source, string channel )
        {
            Console.WriteLine( ">>>>> JOIN EVENT BY " + source.ToString( ) + " INTO " + channel );
        }

        void IAL_QuitEvent( User source, string message )
        {
            Console.WriteLine( ">>>>> QUIT BY " + source.ToString( ) + " MESSAGE " + message );
        }

        void IAL_NicknameChangeEvent( string oldnick, string newnick )
        {
            Console.WriteLine( ">>>>> NICK CHANGE BY " + oldnick + " TO " + newnick );
        }
        #endregion

        void IAL_DataRecievedEvent( string data )
        {
            Console.WriteLine( ">" + data );

            char[ ] colonSeparator = { ':' };

            string messagesource, command, parameters;
            messagesource = command = parameters = "";
            basicParser( data, ref messagesource, ref command, ref parameters );

            User source = new User();

            if ( messagesource != null )
            {
                source = User.newFromString( messagesource );
            }

            switch ( command )
            {
                case "ERROR":
                    if ( parameters.ToLower( ).Contains( ":closing link" ) )
                    {
                        _tcpClient.Close( );
                        _ircReaderThread.Abort( );
                        _ircWriterThread.Abort( );
                    }
                    break;
                case "PING":
                    PingEvent( parameters );
                    break;
                case "NICK":
                    NicknameChangeEvent( source.Nickname, parameters.Substring( 1 ) );
                    break;
                case "MODE":
                    try
                    {
                        string subject = parameters.Split( ' ' )[ 0 ];
                        string flagchanges = parameters.Split( ' ' )[ 1 ];
                        string param = "";
                        if ( parameters.Split( ' ' ).Length > 2 )
                            param = parameters.Split( ' ' )[ 2 ];
                        else
                            param = "";

                        ModeChangeEvent( source, subject, flagchanges, param );
                    }
                    catch ( NullReferenceException ex )
                    {
                        GlobalFunctions.ErrorLog( ex , System.Reflection.MethodInfo.GetCurrentMethod());
                    }
                    break;
                case "QUIT":
                    QuitEvent( source, parameters );
                    break;
                case "JOIN":
                    JoinEvent( source, parameters );
                    break;
                case "PART":
                    PartEvent( source, parameters.Split( ' ' )[ 0 ], parameters.Split( colonSeparator, 2 )[ 1 ] );
                    break;
                case "TOPIC":
                    TopicEvent( source, parameters.Split( ' ' )[ 0 ], parameters.Split( colonSeparator, 2 )[ 1 ] );
                    break;
                case "INVITE":
                    InviteEvent( source, parameters.Split( ' ' )[ 0 ], parameters.Split( ' ' )[ 1 ].Substring( 1 ) );
                    break;
                case "KICK":
                    KickEvent( source, parameters.Split( ' ' )[ 0 ], parameters.Split( ' ' )[ 1 ], parameters.Split( colonSeparator, 2 )[ 1 ] );
                    break;
                case "PRIVMSG":
                    string message = parameters.Split( colonSeparator, 2 )[ 1 ];
                    ASCIIEncoding asc = new ASCIIEncoding( );
                    byte[ ] ctcp = { Convert.ToByte( 1 ) };

                    string destination = parameters.Split( colonSeparator, 2 )[ 0 ].Trim( );
                    if ( destination == this.IrcNickname )
                    {
                        destination = source.Nickname;
                    }

                    if ( message.StartsWith( asc.GetString( ctcp ) ) )
                    {
                        CtcpEvent(
                            source,
                            destination,
                            message.Trim( Convert.ToChar( Convert.ToByte( 1 ) ) )
                            );
                    }
                    else
                    {
                        PrivmsgEvent( source, destination, message.Trim() );
                    }
                    break;

                case "NOTICE":
                    string noticedestination = parameters.Split( colonSeparator, 2 )[ 0 ].Trim( );
                    if ( noticedestination == this.IrcNickname )
                    {
                        noticedestination = source.Nickname;
                    }
                    NoticeEvent( source, noticedestination, parameters.Split( colonSeparator, 2 )[ 1 ] );
                    break;
                case "001":
                    ConnectionRegistrationSucceededEvent( );
                    break;
                case "433":
                    Err_NicknameInUseEvent( );
                    break;
                default:
                    break;
            }
        }

        #region parsers

        private static void basicParser( string line, ref string source, ref string command, ref string parameters )
        {
            char[ ] stringSplitter = { ' ' };
            string[ ] parseBasic;
            if ( line.Substring( 0, 1 ) == ":" )
            {
                parseBasic = line.Split( stringSplitter, 3 );
                source = parseBasic[ 0 ].Substring( 1 );
                command = parseBasic[ 1 ];
                parameters = parseBasic[ 2 ];
            }
            else
            {
                parseBasic = line.Split( stringSplitter, 2 );
                source = null;
                command = parseBasic[ 0 ];
                parameters = parseBasic[ 1 ];
            }
        }
        #endregion

    }
}