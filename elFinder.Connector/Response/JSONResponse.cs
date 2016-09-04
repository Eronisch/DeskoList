using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace elFinder.Connector.Response
{
	public abstract class JSONResponse : IResponse
	{
		#region IResponse Members

		public virtual void Process( System.Web.HttpResponse response )
		{
			string json = JsonConvert.SerializeObject( this, Formatting.Indented );
			response.Write( json );
		}

		#endregion
	}
}
