using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Command
{
	public class UploadCommand : ICommand
	{
		private class uploadArgs
		{
			public string target { get; set; }
		}

		private readonly Service.IVolumeManager _volumeManager;

		public UploadCommand( Service.IVolumeManager volumeManager )
		{
			_volumeManager = volumeManager;
		}

		#region ICommand Members

		public string Name
		{
			get { return "upload"; }
		}

		public Response.IResponse Execute( CommandArgs args )
		{
			var pa = args.As<uploadArgs>();

			if( string.IsNullOrWhiteSpace( pa.target ) )
				return new Response.ErrorResponse( "target not specified" );
			
			// get volume for our target
			var vol = _volumeManager.GetByHash( pa.target );
			if( vol == null )
				return new Response.ErrorResponse( "invalid target" );

			if( args.Files == null || args.Files.Count == 0 )
				return new Response.ErrorResponse( "no files" );
				//return new Response.UploadResponse( new Model.FileModel[ 0 ] );
				
			var savedFiles = vol.SaveFiles( pa.target, args.Files );
			return new Response.UploadResponse( savedFiles );
		}

		#endregion
	}
}
