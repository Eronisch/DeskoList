using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace elFinder.Connector.Response
{
	public class ResizeResponse : JSONResponse
	{
		[JsonProperty( "changed" )]
		public Model.FileModel[] Changed { get; protected set; }

		public ResizeResponse( Model.FileModel[] changedFiles )
		{
			Changed = changedFiles;
		}
	}
}
