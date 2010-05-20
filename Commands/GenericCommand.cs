using System;
namespace helpmebot6.Commands
{
    abstract class GenericCommand
    {

        /// <summary>
        /// Trigger an exectution of the command
        /// </summary>
        /// <param name="source"></param>
        /// <param name="channel"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public CommandResponseHandler run( User source, string channel, string[ ] args )
        {
            string command = this.GetType( ).ToString( );

            Log( "Running command: " + command );

            return accessTest( source, channel, args );
        }

        /// <summary>
        /// Check the access level and then decide what to do.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="channel"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual CommandResponseHandler accessTest( User source, string channel, string[ ] args )
        {
            return reallyRun( source, channel, args );
        }

        /// <summary>
        /// Access granted to command, decide what to do
        /// </summary>
        /// <param name="source"></param>
        /// <param name="channel"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual CommandResponseHandler reallyRun( User source, string channel, string[ ] args )
        {
            Log( "Starting command execution..." );
            CommandResponseHandler crh;
            try
            {
                crh = execute( source, channel, args );
            }
            catch( Exception ex )
            {
                Logger.Instance( ).addToLog( ex.ToString( ), Logger.LogTypes.ERROR );
                crh = new CommandResponseHandler( ex.Message );
            }
            Log( "Command execution complete." );
            return crh;
        }
           
        /// <summary>
        /// Actual command logic
        /// </summary>
        /// <param name="source"></param>
        /// <param name="channel"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected abstract CommandResponseHandler execute( User source, string channel, string[ ] args );

        protected void Log( string message )
        {
            Logger.Instance( ).addToLog( message, Logger.LogTypes.COMMAND );
        }

    }
}
