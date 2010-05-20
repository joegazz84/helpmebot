﻿using System;
using System.Collections.Generic;
using System.Text;

namespace helpmebot6
{
    public class Linker
    {
        Dictionary<string , string> lastLink;

        private static Linker _singleton;

        protected Linker( )
        {
            lastLink = new Dictionary<string , string>( );
            Helpmebot6.irc.PrivmsgEvent += new IAL.PrivmsgEventHandler( irc_PrivmsgEvent );
            Helpmebot6.irc.NoticeEvent += new IAL.PrivmsgEventHandler( irc_PrivmsgEvent );
        }

        void irc_PrivmsgEvent( User source , string destination , string message )
        {
            ParseMessage( message, destination );
        }

        public static Linker Instance( )
        {
            if( _singleton == null )
                _singleton = new Linker( );
            return _singleton;
        }

        public void ParseMessage( string Message, string Channel )
        {
            string newLink = reallyParseMessage( Message );
            if( newLink != "" )
            {
                if( lastLink.ContainsKey( Channel ) )
                {
                    lastLink.Remove( Channel );
                }
                lastLink.Add( Channel , newLink );
                sendLink(Channel, newLink); 
            }
        }

        public string reallyParseMessage( string Message )
        {
            string newLink = "";

            if( ( Message.Contains( "[[" ) && Message.Contains( "]]" ) ) )
            {
                // [[newLink]]

                int startIndex = Message.IndexOf( "[[" );
                int endIndex = Message.IndexOf( "]]", startIndex );

                if( endIndex != -1 )
                {



                    int nextStartIndex = Message.IndexOf( "[[", startIndex + 2, endIndex - startIndex );

                    while( nextStartIndex != -1 )
                    {
                        startIndex = nextStartIndex;
                        nextStartIndex = Message.IndexOf( "[[", startIndex + 2, endIndex - startIndex );
                    }

                    newLink = Message.Substring( startIndex + 2, endIndex - startIndex - 2 );
                }
            }
            if( ( Message.Contains( "{{" ) && Message.Contains( "}}" ) ) )
            {
                int startIndex = Message.IndexOf( "{{" );
                int endIndex = Message.IndexOf( "}}", startIndex );
                if( endIndex != -1 )
                {
                    int nextStartIndex = Message.IndexOf( "{{", startIndex + 2, endIndex - startIndex );

                    while( nextStartIndex != -1 )
                    {
                        startIndex = nextStartIndex;
                        nextStartIndex = Message.IndexOf( "{{", startIndex + 2, endIndex - startIndex );
                    }

                    newLink = "Template:" + Message.Substring( startIndex + 2, endIndex - startIndex - 2 );
                }
            }
            newLink = newLink.Trim( '[' );
            return newLink;
        }

        public string GetLink(string destination)
        {
            return GetLink(destination, false);
        }
        public string GetLink( string destination, bool useSecureServer )
        {
            string link;
            bool success = lastLink.TryGetValue( destination , out link );
            if( success )
            {
                string iwprefix = link.Split( ':' )[ 0 ];

                DAL.Select q = new DAL.Select( "iw_url" );
                q.setFrom( "interwikis" );
                q.addWhere( new DAL.WhereConds( "iw_prefix", iwprefix ) );
                string url = DAL.Singleton( ).executeScalarSelect( q );

                if( url == string.Empty )
                {
                    url = Configuration.Singleton( ).retrieveLocalStringOption( ( useSecureServer ? "wikiSecureUrl" : "wikiUrl" ), destination );
                    return url + antispace( link );
                }
                else
                {
                    return url.Replace( "$1", antispace( string.Join( ":", link.Split( ':' ), 1, link.Split( ':' ).Length - 1 ) ) );
                }
            }
            else
                return "";
        }

        string antispace( string source )
        {
            int currloc = 0;
            string result = "";
            while( currloc < source.Length )
            {
                if( source.Substring( currloc , 1 ) == " " )
                {
                    result += "_";
                }
                else
                {
                    result += source.Substring( currloc , 1 );
                }
                currloc += 1;
            }
            return result;
        }

        void sendLink(string Channel, string Link)
        {
            if (Configuration.Singleton().retrieveLocalStringOption("autoLink", Channel) == "true")
                Helpmebot6.irc.IrcPrivmsg(Channel, GetLink(Link,false));
        }

    }
}
