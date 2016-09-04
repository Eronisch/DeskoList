using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace elFinder.Connector.Response
{
	public class TreeResponse : JSONResponse
	{
		[JsonProperty( "tree" )]
		public Model.DirectoryModel[] Tree { get; protected set; }

		public TreeResponse( Model.DirectoryModel[] tree )
		{
			Tree = tree;
		}
	}
}
