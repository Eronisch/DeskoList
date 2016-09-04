using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace elFinder.Connector.Response
{
	public class RenameResponse: JSONResponse
	{
		[JsonProperty( "added" )]
		public Model.ObjectModel[] Added { get; protected set; }

		[JsonProperty( "removed" )]
		public string[] Removed { get; protected set; }

		public RenameResponse( string oldFileHash, Model.FileModel renamedFile )
		{
			Added = new Model.FileModel[] { renamedFile };
			Removed = new string[] { oldFileHash };
		}

		public RenameResponse( string oldDirHash, Model.DirectoryModel renamedDir )
		{
			Added = new Model.DirectoryModel[] { renamedDir };
			Removed = new string[] { oldDirHash };
		}
	}
}
