using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Command
{
	public class TreeCommand : ICommand
	{
		private class treeArgs
		{
			public string target { get; set; }
		}

		private readonly Service.IVolumeManager _volumeManager;

		public TreeCommand( Service.IVolumeManager volumeManager )
		{
			_volumeManager = volumeManager;
		}

		#region ICommand Members

		public string Name
		{
			get { return "tree"; }
		}

		public Response.IResponse Execute( CommandArgs args )
		{
			var pa = args.As<treeArgs>();

			if( string.IsNullOrWhiteSpace( pa.target ) )
				return new Response.ErrorResponse( "target not specified" );

			// get volume for our target
			var vol = _volumeManager.GetByHash( pa.target );
			// try to get directory for our target
			var cwd = ( vol != null ? vol.GetDirectoryByHash( pa.target ) : null );
			// if we still haven't got volume service, then something is wrong
			if( cwd == null || cwd.IsReadable.IsFalse() )
				return new Response.ErrorResponse( "target dir not found or access denied" );

			var tree = vol.GetSubdirectoriesFlat( cwd, 0 );
			
			return new Response.TreeResponse( tree.ToArray() );
		}

		#endregion
	}
}
