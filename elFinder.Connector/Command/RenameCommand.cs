using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Command
{
	public class RenameCommand : ICommand
	{
		private class renameArgs
		{
			public string target { get; set; }
			public string name { get; set; }
		}

		private readonly Service.IVolumeManager _volumeManager;

		public RenameCommand( Service.IVolumeManager volumeManager )
		{
			_volumeManager = volumeManager;
		}

		#region ICommand Members

		public string Name
		{
			get { return "rename"; }
		}

		public Response.IResponse Execute( CommandArgs args )
		{
			var pa = args.As<renameArgs>();

			if( string.IsNullOrWhiteSpace( pa.target ) )
				return new Response.ErrorResponse( "target not specified" );

			// get volume for our target
			var vol = _volumeManager.GetByHash( pa.target );
			if( vol == null )
				return new Response.ErrorResponse( "invalid target" );
			// rename this object and change first if this is directory or a file
			var dirToChange = vol.GetDirectoryByHash( pa.target );
			if( dirToChange == null )
			{
				var fileToChange = vol.GetFileByHash( pa.target );
				if( fileToChange == null )
					return new Response.ErrorResponse( "target not found" );

				var newFile = vol.RenameFile( fileToChange, pa.name );
				if( newFile == null )
					return new Response.ErrorResponse( "errRename" );
				return new Response.RenameResponse( pa.target, newFile );
			}
			else
			{
				var newDir = vol.RenameDirectory( dirToChange, pa.name );
				if( newDir == null )
					return new Response.ErrorResponse( "errRename" );
				return new Response.RenameResponse( pa.target, newDir );
			}
		}

		#endregion
	}
}
