using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace elFinder.Connector.Response
{
	public class RmResponse : JSONResponse
	{
		[JsonProperty( "removed" )]
		public string[] Removed { get; protected set; }

		public RmResponse( string[] removedHashes )
		{
			Removed = removedHashes;
		}
	}
}
