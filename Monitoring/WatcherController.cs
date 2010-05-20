using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
namespace helpmebot6.Monitoring
{
    /// <summary>
    /// Controls instances of CategoryWatchers for the bot
    /// </summary>
    class WatcherController
    {
        Dictionary<string , CategoryWatcher> watchers;

        protected WatcherController( )
        {
            watchers = new Dictionary<string , CategoryWatcher>( );

            watchers.Add( "helpme", new CategoryWatcher( "Category:Wikipedians looking for help", "helpme", 180 ) );
            watchers.Add( "adminhelp", new CategoryWatcher( "Category:Wikipedians looking for help from administrators", "adminhelp", 180 ) );
            foreach( KeyValuePair<string,CategoryWatcher> item in watchers )
            {
                item.Value.CategoryHasItemsEvent+=new CategoryWatcher.CategoryHasItemsEventHook(CategoryHasItemsEvent);
            }
        }

        // woo singleton
        public static WatcherController Instance( )
        {
            if( _instance == null )
                _instance = new WatcherController( );
            return _instance;
        }
        private static WatcherController _instance;

        public bool isValidKeyword( string keyword )
        {
            return watchers.ContainsKey( keyword );
        }

  

        public string forceUpdate( string key, string destination )
        {
            CategoryWatcher cw;
            if( watchers.TryGetValue( key , out cw ) )
            {
                ArrayList items = cw.doCategoryCheck( );
                return compileMessage( items , key, destination, true );
            }
            else
                return null;
        }

        private void CategoryHasItemsEvent( ArrayList items , string keyword )
        {
            ArrayList channels = new ArrayList( );
            channels.Add( "#wikipedia-en-help" );
            foreach( object[ ] item in channels )
            {
                string message = compileMessage( items, keyword, (string)item[ 0 ], false );
                Helpmebot6.irc.IrcPrivmsg( (string)item[ 0 ] , message );
            }
        }


        private string compileMessage( ArrayList items, string keyword, string destination, bool forceShowAll )
        {   // keywordHasItems: 0: count, 1: plural word(s), 2: items in category
            // keywordNoItems: 0: plural word(s)
            // keywordPlural
            // keywordSingular

            string fakedestination;
                fakedestination = destination;

            string message;

            if( items.Count > 0 )
            {
                string listString = "";
                foreach( string item in items )
                {
                    {
                        listString += "[[" + item + "]]";
                    }


                    listString += ", ";
                }
                listString = listString.TrimEnd( ' ', ',' );
                string pluralString;
                if( items.Count == 1 )
                {
                    pluralString = "user is";
                }
                else
                {
                    pluralString = "users are";
                }
                message = items.Count.ToString( ) + " " + pluralString + " requesting help: " + listString;
            }
            else
            {
                message = "No users requesting help";
            }
            return message;
        }

        private CategoryWatcher getWatcher( string keyword )
        {
            CategoryWatcher cw;
            bool success = watchers.TryGetValue( keyword , out cw );
            if( success )
                return cw;
            else
                return null;
        }

        public Dictionary<string,CategoryWatcher>.KeyCollection getKeywords( )
        {
            return watchers.Keys;
        }
    }
}
