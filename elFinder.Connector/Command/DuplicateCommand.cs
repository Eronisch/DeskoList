using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Command
{
	public class DuplicateCommand : ICommand
	{
		private class duplicateArgs
		{
			public string[] targets { get; set; }
		}

		private readonly Service.IVolumeManager _volumeManager;

		public DuplicateCommand( Service.IVolumeManager volumeManager )
		{
			_volumeManager = volumeManager;
		}

		#region ICommand Members

		public string Name
		{
			get { return "duplicate"; }
		}

		public Response.IResponse Execute( CommandArgs args )
		{
			var pa = args.As<duplicateArgs>();

			if( pa.targets == null || pa.targets.Length == 0 )
				return new Response.ErrorResponse( "source file(s) not specified" );

			IList<Model.ObjectModel> addedElements = new List<Model.ObjectModel>();
			// now copy each element
			foreach( string elemHash in pa.targets )
			{
				var vol = _volumeManager.GetByHash( elemHash );
				if( vol == null )
					continue;
				// check if this is directory or a file
				var dirToCopy = vol.GetDirectoryByHash( elemHash );
				if( dirToCopy == null )
				{
					var fileToCopy = vol.GetFileByHash( elemHash );
					if( fileToCopy == null )
						continue;

					var duplicated = vol.DuplicateFile( fileToCopy );
					if( duplicated != null )
						addedElements.Add( duplicated );
				}
				else
				{
					var duplicated = vol.DuplicateDirectory( dirToCopy );
					if( duplicated != null )
						addedElements.Add( duplicated );
				}
			}

			return new Response.DuplicateResponse( addedElements.ToArray() );
		}

		#endregion
	}
}
