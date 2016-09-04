using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Command
{
	public class TmbCommand : ICommand
	{
		private class tmbArgs
		{
			public string[] targets { get; set; }			
		}

		private readonly Service.IVolumeManager _volumeManager;
		private readonly Service.IImageEditorService _imageEditorService;
		private readonly Config.IConnectorConfig _config;

		public TmbCommand( Service.IVolumeManager volumeManager,
			Service.IImageEditorService imageEditorService, Config.IConnectorConfig config )
		{
			_volumeManager = volumeManager;
			_imageEditorService = imageEditorService;
			_config = config;
		}

		#region ICommand Members

		public string Name
		{
			get { return "tmb"; }
		}

		public Response.IResponse Execute( CommandArgs args )
		{
			var pa = args.As<tmbArgs>();

			if( pa.targets == null || pa.targets.Length == 0 )
				return new Response.ErrorResponse( "target(s) not specified" );

			string thumbsDestPath = _config.LocalFSThumbsDirectoryPath; //TODO: this directory CAN'T be set in CONFIG!
			if( string.IsNullOrWhiteSpace( thumbsDestPath ) )
				return new Response.ErrorResponse( "thumbs target directory not specified in config" );

			Dictionary<string, string> images = new Dictionary<string, string>();
			foreach( var fileHash in pa.targets )
			{
				// get volume for our target
				var vol = _volumeManager.GetByHash( fileHash );
				if( vol == null )
					return new Response.ErrorResponse( "invalid target" );
				var fileToProcess = vol.GetFileByHash( fileHash );
				if( fileToProcess == null )
					return new Response.ErrorResponse( "invalid target" );

				// check if we can generate thumbnail
				if( !_imageEditorService.CanGenerateThumbnail( fileToProcess.Name ) )
					continue;

				string generatedFileName = _imageEditorService.CreateThumbnail( vol.DecodeHashToPath( fileHash ), thumbsDestPath, fileHash,
					_config.ThumbsSize, false );
				if( generatedFileName != null )
				{
					images.Add( fileHash, generatedFileName );
				}
			}

			return new Response.TmbResponse( images );
		}

		#endregion
	}
}
