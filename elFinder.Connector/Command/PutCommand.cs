using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Command
{
	public class PutCommand : ICommand
	{
		private class putArgs
		{
			public string target { get; set; }
			public string content { get; set; }
		}

		private readonly Service.IVolumeManager _volumeManager;

		public PutCommand( Service.IVolumeManager volumeManager )
		{
			_volumeManager = volumeManager;
		}

		#region ICommand Members

		public string Name
		{
			get { return "put"; }
		}

		public Response.IResponse Execute( CommandArgs args )
		{
			var pa = args.As<putArgs>();

			if( string.IsNullOrWhiteSpace( pa.target ) )
				return new Response.ErrorResponse( "target not specified" );

			// get volume for our target
			var vol = _volumeManager.GetByHash( pa.target );
			if( vol == null )
				return new Response.ErrorResponse( "invalid target" );

			var fileToModify = vol.GetFileByHash( pa.target );
			if( fileToModify == null )
				return new Response.ErrorResponse( "invalid target" );

			Model.FileModel modifiedFile = vol.SetTextFileContent( fileToModify, pa.content );
			if( modifiedFile == null )
				return new Response.ErrorResponse( "error setting file content" );
			return new Response.PutResponse( new Model.FileModel[]{ modifiedFile } );
		}

		#endregion
	}
}
