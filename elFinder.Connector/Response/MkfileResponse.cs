using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace elFinder.Connector.Response
{
	public class MkfileResponse : JSONResponse
	{
		[JsonProperty( "added" )]
		public Model.FileModel[] Added { get; protected set; }

		public MkfileResponse( Model.FileModel createdFile )
		{
			Added = new Model.FileModel[] { createdFile };
		}
	}
}
