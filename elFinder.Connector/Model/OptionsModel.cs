using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace elFinder.Connector.Model
{
	public class OptionsModel
	{
		[JsonProperty("archivers")]
		public string[] Archivers {get; private set;}

		[JsonProperty("copyOverwrite")]
		public int CopyOverwrite {get; private set;}

		[JsonProperty("disabled")]
		public string[] Disabled {get; private set;}

		[JsonProperty("path")]
		public string Path {get; private set;}

		[JsonProperty("separator")]
		public string Separator {get; private set;}

		[JsonProperty("url")]
		public string Url {get; private set;}

		[JsonProperty("tmbUrl")]
		public string ThumbnailsUrl {get; private set;}

		public OptionsModel( string path, string baseUrl, string baseThumbsUrl )
		{
			Archivers = new string[0];
			CopyOverwrite = 1;
			Disabled = new string[0];
			Path = path;
			Separator = System.IO.Path.DirectorySeparatorChar.ToString();
			Url = baseUrl;
			ThumbnailsUrl = baseThumbsUrl;
		}
	}
}
