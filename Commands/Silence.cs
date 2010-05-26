﻿using System;
using System.Collections.Generic;
using System.Text;

namespace helpmebot6.Commands
{
    class Silence : GenericCommand
    {
        public Silence( )
        {

        }

        protected override CommandResponseHandler execute( User source , string channel , string[ ] args )
        {
            Logger.Instance( ).addToLog( "Method:" + System.Reflection.MethodInfo.GetCurrentMethod( ).DeclaringType.Name + System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Logger.LogTypes.DNWB );

            bool global = false;


            if( args.Length > 0 )
            {
                if( args[ 0 ].ToLower() == "@global" )
                {
                    global = true;
                    GlobalFunctions.popFromFront( ref args );
                }
            }
            bool oldValue;

            if( !global )
            {
                oldValue = bool.Parse( Configuration.Singleton( ).retrieveLocalStringOption( "silence" , channel ) );
            }
            else
            {
                oldValue = bool.Parse( Configuration.Singleton( ).retrieveGlobalStringOption( "silence" ) );
            }

            if( args.Length > 0 )
            {
                string newValue = "global";
                switch( args[0].ToLower() )
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
                if( newValue == oldValue.ToString().ToLower() )
                {
                    return new CommandResponseHandler( Configuration.Singleton( ).GetMessage( "no-change" ) , CommandResponseDestination.PRIVATE_MESSAGE );
                }
                else
                {
                    if( newValue == "global" )
                    {
                        Configuration.Singleton( ).deleteLocalOption( "silence" , channel );
                        return new CommandResponseHandler( Configuration.Singleton( ).GetMessage( "defaultConfig" ) , CommandResponseDestination.PRIVATE_MESSAGE );

                    }
                    else
                    {
                        if( !global )
                            Configuration.Singleton( ).setLocalOption( "silence", channel, newValue );
                        else
                        {
                            if(source.AccessLevel >= User.userRights.Superuser)
                                Configuration.Singleton( ).setGlobalOption( "silence", newValue );
                        }
                        return new CommandResponseHandler( Configuration.Singleton( ).GetMessage( "done" ) , CommandResponseDestination.PRIVATE_MESSAGE );

                    }
                }
            }
            string[ ] mP = { "silence" , 1.ToString() , args.Length.ToString() };
            return new CommandResponseHandler( Configuration.Singleton( ).GetMessage( "notEnoughParameters" , mP ) , CommandResponseDestination.PRIVATE_MESSAGE );
        }
    }
}