using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace elFinder.Connector.Config
{
	public interface IConnectorConfig
	{
		string ApiVersion { get; }
		string LocalFSRootDirectoryPath { get; }
		string LocalFSThumbsDirectoryPath { get; }

		string DuplicateFilePattern { get; }
		string DuplicateDirectoryPattern { get; }

		string RootDirectoryName { get; }
		string DefaultVolumeName { get; }
		string UploadMaxSize { get; }
		string BaseUrl { get; }
		string BaseThumbsUrl { get; }
		Size ThumbsSize { get; }

		int MaxTreeLevel { get; }
	}
}
