using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace elFinder.Connector.Response
{
	public class TmbResponse : JSONResponse
	{
		[JsonProperty( "images" )]
		public Dictionary<string, string> Images { get; private set; }

		public TmbResponse( Dictionary<string, string> images )
		{
			Images = images;
		}
	}
}
