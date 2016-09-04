using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Command
{
	public class GetCommand : ICommand
	{
		private class getArgs
		{
			public string target { get; set; }			
		}

		private readonly Service.IVolumeManager _volumeManager;

		public GetCommand( Service.IVolumeManager volumeManager )
		{
			_volumeManager = volumeManager;
		}

		#region ICommand Members

		public string Name
		{
			get { return "get"; }
		}

		public Response.IResponse Execute( CommandArgs args )
		{
			var pa = args.As<getArgs>();

			if( string.IsNullOrWhiteSpace( pa.target ) )
				return new Response.ErrorResponse( "target not specified" );

			// get volume for our target
			var vol = _volumeManager.GetByHash( pa.target );
			if( vol == null )
				return new Response.ErrorResponse( "invalid target" );

			var fileToGet = vol.GetFileByHash( pa.target );
			if( fileToGet == null )
				return new Response.ErrorResponse( "invalid target" );

			var content = vol.GetTextFileContent( fileToGet );
			if( content == null )
				return new Response.ErrorResponse( "error getting file content" );
			return new Response.GetResponse( content );
		}

		#endregion
	}
}
