using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace elFinder.Connector.Response
{
	public class InitDirectoryResponse : DirectoryResponse
	{
		[JsonProperty( "api" )]
		public string Api { get; protected set; }

		[JsonProperty( "netDrivers" )]
		public string[] NetDrivers { get; protected set; }

		[JsonProperty( "uplMaxSize" )]
		public string UploadMaxSize { get; protected set; }

		public InitDirectoryResponse( string apiVersion,
			string[] netDrivers, string uploadMaxSize, //TODO: change to int in bytes and handle conversion here
			Model.DirectoryModel cwd, 
			Model.ObjectModel[] subItems,
			Model.OptionsModel opts )
			: base( cwd, subItems, opts )
		{
			Api = apiVersion;
			NetDrivers = netDrivers;
			UploadMaxSize = uploadMaxSize;
		}
	}
}
