using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Command
{
	public class ParentsCommand : ICommand
	{
		private class parentsArgs
		{
			public string target { get; set; }
		}

		private readonly Service.IVolumeManager _volumeManager;

		public ParentsCommand( Service.IVolumeManager volumeManager )
		{
			_volumeManager = volumeManager;
		}

		#region ICommand Members

		public string Name
		{
			get { return "parents"; }
		}

		public Response.IResponse Execute( CommandArgs args )
		{
			var pa = args.As<parentsArgs>();

			if( string.IsNullOrWhiteSpace( pa.target ) )
				return new Response.ErrorResponse( "target not specified" );

			// get volume for our target
			var vol = _volumeManager.GetByHash( pa.target );
			// try to get directory for our target
			var cwd = ( vol != null ? vol.GetDirectoryByHash( pa.target ) : null );
			// if we still haven't got volume service, then something is wrong
			if( cwd == null || cwd.IsReadable.IsFalse() )
				return new Response.ErrorResponse( "target dir not found or access denied" );

			HashSet<Model.DirectoryModel> tree = new HashSet<Model.DirectoryModel>();
			tree.Add( cwd );
			var rootDir = vol.GetRootDirectory();
			// now get parent untill we reach root
			while( cwd != null && cwd.Hash != rootDir.Hash )
			{
				if( cwd.ParentHash == null )
					break;
				var parentDir = vol.GetDirectoryByHash( cwd.ParentHash );
				if( parentDir == null )
					return new Response.ErrorResponse( "error getting parent dir: not found or access denied" );
				tree.Add( parentDir );
				var subDirs = vol.GetSubdirectoriesFlat( parentDir, 0 );
				foreach( var sd in subDirs )
					tree.Add( sd );
				cwd = parentDir;
			}
			return new Response.TreeResponse( tree.ToArray() );
		}

		#endregion
	}
}
