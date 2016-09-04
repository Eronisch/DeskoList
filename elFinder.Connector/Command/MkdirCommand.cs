using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Command
{
	public class MkdirCommand: ICommand
	{
		private class mkdirArgs
		{
			public string target { get; set; }
			public string name { get; set; }
		}

		private readonly Service.IVolumeManager _volumeManager;

		public MkdirCommand( Service.IVolumeManager volumeManager )
		{
			_volumeManager = volumeManager;
		}

		#region ICommand Members

		public string Name
		{
			get { return "mkdir"; }
		}

		public Response.IResponse Execute( CommandArgs args )
		{
			var pa = args.As<mkdirArgs>();

			if( string.IsNullOrWhiteSpace( pa.target ) )
				return new Response.ErrorResponse( "target not specified" );

			// get volume for our target
			var vol = _volumeManager.GetByHash( pa.target );
			// try to get directory for our target
			var cwd = ( vol != null ? vol.GetDirectoryByHash( pa.target ) : null );
			// if we still haven't got volume service, then something is wrong
			if( cwd == null || cwd.IsReadable.IsFalse() )
				return new Response.ErrorResponse( "target dir not found or access denied" );

			var created = vol.CreateDirectory( cwd, pa.name );
			if( created == null )
				return new Response.ErrorResponse( "errMkdir" );

			return new Response.MkdirResponse( created );
		}

		#endregion
	}
}
