using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace elFinder.Connector.Response
{
	public class GetResponse : JSONResponse
	{
		[JsonProperty("content")]
		public string TextContent { get; private set; }

		public GetResponse( string textContent )
		{
			TextContent = textContent;
		}
	}
}
