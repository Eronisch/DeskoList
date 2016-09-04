using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace elFinder.Connector.Model
{
	public class TmbConverter : JsonConverter
	{
		public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
		{
			if( value == null )
				return;
			string valStr = value.ToString();
			if( !string.IsNullOrWhiteSpace( valStr ) )
			{
				if( valStr == FileModel.ThumbnailIsSupported )
					writer.WriteValue( 1 );
				else
					writer.WriteValue( valStr );
			}
		}

		public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
		{
			throw new NotImplementedException();
		}

		public override bool CanConvert( Type objectType )
		{
			return objectType == typeof( string );
		}
	}

	[JsonObject]
	public class FileModel : ObjectModel
	{
		public const string ThumbnailIsSupported = "1";

		[JsonProperty( "tmb", NullValueHandling=NullValueHandling.Ignore )]
		[JsonConverter(typeof(TmbConverter) )]
		public string Thumbnail { get; set; }

		public FileModel( string name, string thumbnailName, string hash, long size, string parentHash, 
			DateTime dateModified, string volumeId, bool isReadable, bool isWritable, bool isLocked )
		{
			Name = name;
			Hash = hash;
			ParentHash = parentHash;
			ModifiedTimeStamp = getTS( dateModified );
			VolumeID = volumeId;
			IsReadable = isReadable.ToInt();
			IsWriteable = isWritable.ToInt();
			IsLocked = isLocked.ToInt();

			Mime = MimeTypes.GetContentType( name );
			Size = size;
			Thumbnail = thumbnailName;
		}
	}
}
