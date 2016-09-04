using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Command
{
	public class MkfileCommand: ICommand
	{
		private class mkfileArgs
		{
			public string target { get; set; }
			public string name { get; set; }
		}

		private readonly Service.IVolumeManager _volumeManager;

		public MkfileCommand( Service.IVolumeManager volumeManager )
		{
			_volumeManager = volumeManager;
		}

		#region ICommand Members

		public string Name
		{
			get { return "mkfile"; }
		}

		public Response.IResponse Execute( CommandArgs args )
		{
			var pa = args.As<mkfileArgs>();

			if( string.IsNullOrWhiteSpace( pa.target ) )
				return new Response.ErrorResponse( "target not specified" );

			// get volume for our target
			var vol = _volumeManager.GetByHash( pa.target );
			// try to get directory for our target
			var cwd = ( vol != null ? vol.GetDirectoryByHash( pa.target ) : null );
			// if we still haven't got volume service, then something is wrong
			if( cwd == null || cwd.IsReadable.IsFalse() )
				return new Response.ErrorResponse( "target dir not found or access denied" );

			var created = vol.CreateFile( cwd, pa.name );
			if( created == null )
				return new Response.ErrorResponse( "errMkdir" );

			return new Response.MkfileResponse( created );
		}

		#endregion
	}
}
