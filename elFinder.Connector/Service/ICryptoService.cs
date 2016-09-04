using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Service
{
	public interface ICryptoService
	{
		string Encode( string input );
		string Decode( string hash );
	}
}
