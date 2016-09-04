using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace elFinder.Connector.Response
{
	public class UploadResponse : JSONResponse
	{
		[JsonProperty( "added" )]
		public Model.FileModel[] Added { get; protected set; }

		public UploadResponse( Model.FileModel[] addedFiles )
		{
			Added = addedFiles;
		}
	}
}
