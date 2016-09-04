using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace elFinder.Connector.Response
{
	public class ErrorResponse : JSONResponse
	{
		[JsonProperty("error")]
		public string Error { get; private set; }

		public ErrorResponse( string errorMsg )
		{
			Error = errorMsg;
		}
	}
}
