using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Command
{
	public class RmCommand : ICommand
	{
		private class rmArgs
		{
			public string[] targets { get; set; }
		}

		private readonly Service.IVolumeManager _volumeManager;

		public RmCommand( Service.IVolumeManager volumeManager )
		{
			_volumeManager = volumeManager;
		}

		#region ICommand Members

		public string Name
		{
			get { return "rm"; }
		}

		public Response.IResponse Execute( CommandArgs args )
		{
			var ra = args.As<rmArgs>();

			if( ra.targets == null || ra.targets.Length == 0 )
				return new Response.ErrorResponse( "target(s) not specified" );

			IList<string> removeHashes = new List<string>();
			foreach( var toRemove in ra.targets )
			{
				var vol = _volumeManager.GetByHash( toRemove );
				if( vol == null )
					continue;

				var dirToRemove = vol.GetDirectoryByHash( toRemove );
				if( dirToRemove == null )
				{
					var fileToRemove = vol.GetFileByHash( toRemove );
					if( fileToRemove == null )
						continue;

					if( vol.DeleteFile( fileToRemove ) )
						removeHashes.Add( toRemove );
				}
				else
				{
					if( vol.DeleteDirectory( dirToRemove ) )
						removeHashes.Add( toRemove );
				}
			}

			return new Response.RmResponse( removeHashes.ToArray() );
		}

		#endregion
	}
}
