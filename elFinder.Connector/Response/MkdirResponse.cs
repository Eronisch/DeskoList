using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace elFinder.Connector.Response
{
	public class MkdirResponse : JSONResponse
	{
		[JsonProperty( "added" )]
		public Model.DirectoryModel[] Added { get; protected set; }

		public MkdirResponse( Model.DirectoryModel createdDirectory )
		{
			Added = new Model.DirectoryModel[] { createdDirectory };
		}
	}
}
