using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace elFinder.Connector.Model
{
	[JsonObject]
	public abstract class ObjectModel : IEquatable<ObjectModel>
	{
		[JsonProperty( "name" )]
		public string Name { get; protected set; }

		[JsonProperty( "hash" )]
		public string Hash { get; protected set; }

		[JsonProperty( "phash", NullValueHandling = NullValueHandling.Ignore )]
		public string ParentHash { get; protected set; }

		[JsonProperty( "mime" )]
		public string Mime { get; protected set; }

		[JsonProperty( "ts" )]
		public int ModifiedTimeStamp { get; protected set; }

		[JsonProperty( "volumeid" )]
		public string VolumeID { get; protected set; }

		[JsonProperty( "dirs", NullValueHandling=NullValueHandling.Ignore )]
		public int? Dirs { get; protected set; }

		[JsonProperty( "read" )]
		public int IsReadable { get; protected set; }

		[JsonProperty( "size" )]
		public long Size { get; protected set; }

		[JsonProperty( "write" )]
		public int IsWriteable { get; protected set; }

		[JsonProperty( "locked" )]
		public int IsLocked { get; protected set; }

		protected int getTS( DateTime targetDate )
		{
			return (int)( targetDate.ToUnixTicks() / 1000 );
		}

		#region IEquatable<ObjectModel> Members

		public bool Equals( ObjectModel other )
		{
			if( other == null )
				return false;
			return this.Hash.Equals( other.Hash, StringComparison.OrdinalIgnoreCase );
		}

		#endregion

		public override int GetHashCode()
		{
			return Hash.GetHashCode();
		}

		public override bool Equals( object obj )
		{
			var other = obj as ObjectModel;
			if( other == null )
				return false;

			return this.Hash.Equals( other.Hash, StringComparison.OrdinalIgnoreCase );
		}
	}
}
