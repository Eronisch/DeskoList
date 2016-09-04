using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace elFinder.Connector.Response
{
	public class DirectoryResponse : JSONResponse
	{
		[JsonProperty("cwd")]
		public Model.DirectoryModel CWD { get; protected set; }
		
		[JsonProperty( "files" )]
		public Model.ObjectModel[] SubItems { get; protected set; }

		[JsonProperty( "options", NullValueHandling=NullValueHandling.Ignore )]
		public Model.OptionsModel Options { get; protected set; }

		public DirectoryResponse( Model.DirectoryModel cwd, Model.ObjectModel[] subItems,
			Model.OptionsModel opts )
		{
			CWD = cwd;
			SubItems = subItems;
			Options = opts;
		}
	}
}
