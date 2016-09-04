using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;

namespace elFinder.Connector
{
	public static class Utils
	{
		public static object ConvertTo( object objToConvert, Type destType )
		{
			if( destType.IsGenericType
				&& ( destType.GetGenericTypeDefinition() == typeof( Nullable<> ) ) )
			{
				if( objToConvert == null )
					return null;
				else
					return ConvertTo( objToConvert, Nullable.GetUnderlyingType( destType ) );
			}
			else if( destType.IsArray )
			{
				// array elements are separated by comma (,)
				string[] elems = objToConvert.ToString().Split( ',' );
				Type arrayElemType = destType.GetElementType();
				Array retArray = Array.CreateInstance( arrayElemType, elems.Length );
				for( int idx = 0; idx < elems.Length; ++idx)
				{
					retArray.SetValue( ConvertTo( elems[ idx ], arrayElemType ), idx );
				}
				return retArray;
			}
			else if( destType.IsEnum )
			{
				return Enum.Parse( destType, objToConvert.ToString() );
			}
			else
			{
				return Convert.ChangeType( objToConvert, destType );
			}
		}

		public static bool IsTrue( this int? val )
		{
			return ( val != null && val.Value > 0 );
		}

		public static bool IsFalse( this int? val )
		{
			return ( val == null || val.Value == 0 );
		}

		public static bool IsTrue( this int val )
		{
			return ( val > 0 );
		}

		public static bool IsFalse( this int val )
		{
			return ( val == 0 );
		}

		public static int ToInt( this bool val )
		{
			return val ? 1 : 0;
		}

		public static ImageFormat GetImageFormatByExt( string ext )
		{
			if( ext == "jpg" || ext == "jpeg" )
				return ImageFormat.Jpeg;
			if( ext == "png" )
				return ImageFormat.Png;
			if( ext == "gif" )
				return ImageFormat.Gif;
			if( ext == "tiff" )
				return ImageFormat.Tiff;
			if( ext == "bmp" )
				return ImageFormat.Bmp;
			return ImageFormat.Jpeg;
		}

		public static readonly DateTime Epoch = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );

		/// <summary>
		/// returns the number of milliseconds since Epoch - Jan 1, 1970 (useful for converting C# dates to JS dates)
		/// </summary>
		public static double ToUnixTicks( this DateTime time )
		{
			time = time.ToUniversalTime();
			var diff = new TimeSpan( time.Ticks - Epoch.Ticks );
			return Math.Round( diff.TotalMilliseconds, 0 );
		}

		public static DateTime FromUnixTicks( double ticks, DateTimeKind kind )
		{
			var result = Epoch.AddMilliseconds( ticks );

			return kind == DateTimeKind.Utc
					   ? result
					   : result.ToLocalTime();
		}
	}
}
