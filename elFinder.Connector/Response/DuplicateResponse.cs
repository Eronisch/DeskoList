using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace elFinder.Connector.Response
{
	public class DuplicateResponse: JSONResponse
	{
		[JsonProperty( "added" )]
		public Model.ObjectModel[] Added { get; protected set; }

		public DuplicateResponse( Model.ObjectModel[] added )
		{
			Added = added;
		}
	}
}
