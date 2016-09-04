using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using elFinder.Connector.Service;

namespace elFinder.Connector.Model
{
	[JsonObject]
	public class DirectoryModel : ObjectModel
	{
		public DirectoryModel( string name, string hash, string parentHash, bool hasSubDirectories, DateTime dateModified,
			string volumeId, bool isReadable, bool isWritable, bool isLocked )
		{
			Name = name;
			Hash = hash;
			ParentHash = parentHash;
			ModifiedTimeStamp = getTS( dateModified );
			VolumeID = volumeId;
			IsReadable = isReadable.ToInt();
			IsWriteable = isWritable.ToInt();
			IsLocked = isLocked.ToInt();

			Dirs = hasSubDirectories.ToInt();
			Mime = "directory";
			Size = 0;
		}
	}
}
