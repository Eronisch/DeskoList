using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using elFinder.Connector.Model;

namespace elFinder.Connector.Command
{
	public class OpenCommand : ICommand
	{
		private class openArgs
		{
			public int? init { get; set; }
			public string target { get; set; }
			public int? tree { get; set; }
		}

		private readonly Config.IConnectorConfig _config;
		private readonly Service.IVolumeManager _volumeManager;

		public OpenCommand( Config.IConnectorConfig config, Service.IVolumeManager volumeManager )
		{
			_config = config;
			_volumeManager = volumeManager;
		}

		#region ICommand Members

		public string Name
		{
			get { return "open"; }
		}

		public Response.IResponse Execute( CommandArgs args )
		{
			var oa = args.As<openArgs>();

			if( string.IsNullOrWhiteSpace( oa.target ) && oa.init.IsFalse() )
				return new Response.ErrorResponse( "target not specified" );

			// get volume for our target
			var vol = _volumeManager.GetByHash( oa.target );
			// try to get directory for our target
			var cwd = ( vol != null ? vol.GetDirectoryByHash( oa.target ) : null );

			if( (cwd == null || cwd.IsReadable.IsFalse() ) && oa.init.IsTrue() )
			{
				// get default volume
				vol = _volumeManager.DefaultVolume;
				if( vol != null )
					cwd = vol.GetRootDirectory();
			}

			// if we still haven't got volume service, then something is wrong
			if( cwd == null || cwd.IsReadable.IsFalse() )
				return new Response.ErrorResponse( "target dir not found or access denied" );

			var subItems = new List<Model.ObjectModel>();

			// get sub directories
			if( oa.tree.IsTrue() )
				subItems.AddRange( vol.GetSubdirectoriesFlat( cwd ) );
			else
				subItems.AddRange( vol.GetSubdirectoriesFlat( cwd, 0 ) );

			// get files in our CWD
			subItems.AddRange( vol.GetFiles( cwd ) );
			
			if( oa.init.IsTrue() )
			{
				return new Response.InitDirectoryResponse( _config.ApiVersion,
					new string[0], //TODO
					_config.UploadMaxSize,
					cwd, 
					subItems.ToArray(),
					new Model.OptionsModel( vol.GetPathToRoot( cwd ), _config.BaseUrl, _config.BaseThumbsUrl ) );
			}
			else
			{
				return new Response.DirectoryResponse( cwd,
					subItems.ToArray(),
					new Model.OptionsModel( vol.GetPathToRoot( cwd ), _config.BaseUrl, _config.BaseThumbsUrl ) );
			}
		}

		#endregion
	}
}
