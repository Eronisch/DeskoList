using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Service
{
	public class DefaultVolumeManager : IVolumeManager
	{
		private const string volumePrefix = "v";

		private readonly Config.IConnectorConfig _config;

		private Dictionary<string, IVolume> _hashedVolumes;

		public IVolume DefaultVolume
		{
			get;
			private set;
		}

		public DefaultVolumeManager( Config.IConnectorConfig config, IEnumerable<IVolume> volumes )
		{
			_config = config;
			// validate config
			if( string.IsNullOrWhiteSpace( _config.DefaultVolumeName ) )
				throw new ArgumentNullException( "Default volume name not specified in configuration, please specify one" );

			_hashedVolumes = new Dictionary<string,IVolume>();
			var volList = volumes.ToList();
			for( int i = 0; i < volList.Count; ++i )
			{
				string vId = volumePrefix + i + "_";
				volList[ i ].Id = vId; //TODO: right now we have to set volume ID like this, but it would be better if we could not have setter for ID in IVolume
				_hashedVolumes.Add( vId, volList[ i ] );
				// check if this is our default volume
				if( volList[ i ].Name.Equals( _config.DefaultVolumeName, StringComparison.OrdinalIgnoreCase ) )
					DefaultVolume = volList[ i ];
			}
			if( DefaultVolume == null )
				throw new InvalidOperationException( "Default volume with name " + _config.DefaultVolumeName + " not found" );
		}

		#region IVolumeServiceManager Members

		public IEnumerable<IVolume> VolumeServices
		{
			get { return _hashedVolumes.Values; }
		}

		public IVolume GetByHash( string hash )
		{
			if( _hashedVolumes == null || _hashedVolumes.Count == 0 || string.IsNullOrWhiteSpace( hash ) )
				return null;
			string foundKey = _hashedVolumes.Keys.FirstOrDefault( x => hash.StartsWith( x, StringComparison.OrdinalIgnoreCase ) );
			if( string.IsNullOrWhiteSpace( foundKey ) )
				return null;
			return _hashedVolumes[ foundKey ];
		}

		#endregion
	}
}
