using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace elFinder.Connector.Response
{
	public interface IResponse
	{
		void Process( HttpResponse response );
	}
}
