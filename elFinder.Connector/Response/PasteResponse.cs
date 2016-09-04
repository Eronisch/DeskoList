using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace elFinder.Connector.Response
{
	public class PasteResponse : JSONResponse
	{
		[JsonProperty( "added" )]
		public Model.ObjectModel[] Added { get; protected set; }
		
		[JsonProperty( "removed" )]
		public string[] Removed { get; protected set; }

		public PasteResponse( Model.ObjectModel[] added, string[] removed )
		{
			Added = added;
			Removed = removed;
		}
	}
}
