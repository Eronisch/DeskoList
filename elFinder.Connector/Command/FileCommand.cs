using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Command
{
	public class FileCommand : ICommand
	{
		private class fileArgs
		{
			public string target { get; set; }			
		}

		private readonly Service.IVolumeManager _volumeManager;

		public FileCommand( Service.IVolumeManager volumeManager )
		{
			_volumeManager = volumeManager;
		}

		#region ICommand Members

		public string Name
		{
			get { return "file"; }
		}

		public Response.IResponse Execute( CommandArgs args )
		{
			var pa = args.As<fileArgs>();

			if( string.IsNullOrWhiteSpace( pa.target ) )
				return new Response.ErrorResponse( "target not specified" );

			// get volume for our target
			var vol = _volumeManager.GetByHash( pa.target );
			if( vol == null )
				return new Response.ErrorResponse( "invalid target" );
			// ensure that the file is valid
			var fileToGet = vol.GetFileByHash( pa.target );
			if( fileToGet == null )
				return new Response.ErrorResponse( "invalid target" );
			// now get path
			string filePath = vol.DecodeHashToPath( pa.target );
			return new Response.BinaryFileResponse( filePath );
		}

		#endregion
	}
}
