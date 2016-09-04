using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Service
{
	public class Base64CryptoService : ICryptoService
	{
		#region ICryptoService Members

		public string Encode( string input )
		{
			byte[] bytes = Encoding.UTF8.GetBytes( input );
			string encoded = Convert.ToBase64String( bytes, Base64FormattingOptions.None );
			// need to replace some special chars to make whole string compatible
			encoded = encoded.Replace( '+', '«' )
							.Replace( '/', '»' )
							.Replace( '=', '§' );
			return encoded;
		}

		public string Decode( string hash )
		{
			hash = hash.Replace( '«', '+' )
							.Replace( '»', '/' )
							.Replace( '§', '=' );
			byte[] bytes = Convert.FromBase64String( hash );
			string decoded = Encoding.UTF8.GetString( bytes );
			return decoded;
		}

		#endregion
	}
}
