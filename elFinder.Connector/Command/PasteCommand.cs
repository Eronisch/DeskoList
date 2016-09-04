using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Command
{
	public class PasteCommand : ICommand
	{
		private class pasteArgs
		{
			public string src { get; set; }
			public string dst { get; set; }
			public int cut { get; set; }
			public string[] targets { get; set; }
		}

		private readonly Service.IVolumeManager _volumeManager;

		public PasteCommand( Service.IVolumeManager volumeManager )
		{
			_volumeManager = volumeManager;
		}

		#region ICommand Members

		public string Name
		{
			get { return "paste"; }
		}

		public Response.IResponse Execute( CommandArgs args )
		{
			var pa = args.As<pasteArgs>();

			if( pa.targets == null || pa.targets.Length == 0 )
				return new Response.ErrorResponse( "source file(s) not specified" );
			if( string.IsNullOrWhiteSpace( pa.dst ) )
				return new Response.ErrorResponse( "destination directory not specified" );

			var dstVol = _volumeManager.GetByHash( pa.dst );
			if( dstVol == null )
				return new Response.ErrorResponse( "invalid target" );

			string dstPath = dstVol.DecodeHashToPath( pa.dst );

			bool cut = pa.cut.IsTrue();
			IList<Model.ObjectModel> changedElements = new List<Model.ObjectModel>();
			IList<string> removedElements = new List<string>();
			// now copy each element
			foreach( string elemHash in pa.targets )
			{
				var vol = _volumeManager.GetByHash( elemHash );
				if( vol == null || dstVol != vol )
					continue;
				// check if this is directory or a file
				var dirToCopy = vol.GetDirectoryByHash( elemHash );
				if( dirToCopy == null )
				{
					var fileToCopy = vol.GetFileByHash( elemHash );
					if( fileToCopy == null )
						continue;

					var copied = vol.CopyFile( fileToCopy, dstPath, cut );
					if( copied != null )
					{
						changedElements.Add( copied );
						if( cut ) // if we are cutting element, then it was removed (or at least should be)
							removedElements.Add( elemHash );
					}
				}
				else
				{
					var copied = vol.CopyDirectory( dirToCopy,dstPath, cut );
					if( copied != null )
					{
						changedElements.Add( copied );
						if( cut ) // if we are cutting element, then it was removed (or at least should be)
							removedElements.Add( elemHash );
					}
				}
			}
			
			return new Response.PasteResponse( changedElements.ToArray(), removedElements.ToArray() );
		}

		#endregion
	}
}
