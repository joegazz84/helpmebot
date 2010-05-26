﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using helpmebot6.Threading;
namespace helpmebot6.UdpListener
{
    public class UDPListener:IThreadedSystem
    {
        public UDPListener( int port )
        {
            key = Configuration.Singleton( ).retrieveGlobalStringOption( "udpKey" );
            udpClient = new UdpClient( port );
            listenerThread = new Thread( new ThreadStart( threadMethod ) );
            RegisterInstance( );
            listenerThread.Start( );
        }

        
        Thread listenerThread;
        UdpClient udpClient;
        string key;

        private void threadMethod( )
        {
            Logger.Instance( ).addToLog( "Method:" + System.Reflection.MethodInfo.GetCurrentMethod( ).DeclaringType.Name + System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Logger.LogTypes.DNWB );

            try
            {
                while( true )
                {
                    IPEndPoint ipep = new IPEndPoint( IPAddress.Any, 0 );
                    if( udpClient.Available != 0 )
                    {
                        byte[ ] datagram = udpClient.Receive( ref ipep );

                        BinaryFormatter bf = new BinaryFormatter( );
                        UdpMessage recievedMessage = (UdpMessage)bf.Deserialize( new MemoryStream( datagram ) );

                        byte[ ] bm = ASCIIEncoding.ASCII.GetBytes( recievedMessage.Message + key );
                        byte[ ] bh = MD5.Create( ).ComputeHash( bm );
                        string hash = ASCIIEncoding.ASCII.GetString( bh );
                        if( hash == recievedMessage.Hash )
                        {
                            Helpmebot6.irc.SendRawLine( "PRIVMSG " + recievedMessage.Message );
                        }
                    }
                    else
                    {
                        Thread.Sleep( 500 );
                    }
                }
            }
            catch( ThreadAbortException )
            {
                EventHandler temp = ThreadFatalError;
                if( temp != null )
                {
                    temp( this, new EventArgs( ) );
                }
            }
        }

        #region IThreadedSystem Members

        public void Stop( )
        {
            listenerThread.Abort( );
        }

        public void RegisterInstance( )
        {
            ThreadList.instance( ).register( this );
        }

        public string[ ] getThreadStatus( )
        {
            string[ ] statuses = { this.listenerThread.ThreadState.ToString() };
            return statuses;
        }

        public event EventHandler ThreadFatalError;

        #endregion
    }
}