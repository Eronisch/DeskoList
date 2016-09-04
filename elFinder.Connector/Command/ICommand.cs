using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.Linq.Expressions;

namespace elFinder.Connector.Command
{
	public class CommandArgs
	{
		public NameValueCollection Parameters { get; private set; }
		public IList<HttpPostedFile> Files { get; private set; }

		public CommandArgs( NameValueCollection parameters, HttpFileCollection files )
		{
			Parameters = parameters;
			Files = new List<HttpPostedFile>();
			for( int i = 0; i < files.Count; ++i )
			{
				if( files[ i ] == null )
					continue;
				Files.Add( files[ i ] );
			}
		}

		public FinalArgType As<FinalArgType>()
			where FinalArgType : new()
		{
			var finalArgs = new FinalArgType();
			Array.ForEach( finalArgs.GetType().GetProperties(),
				pi => 
					{
						// handle array in a special way
						string parameterName = pi.Name;
						if( pi.PropertyType.IsArray )
							parameterName += "[]";
						pi.SetValue( finalArgs, Utils.ConvertTo( Parameters[ parameterName ], pi.PropertyType ), null );
					}
					);
			return finalArgs;
		}
	}

	public interface ICommand
	{
		string Name { get; }
		Response.IResponse Execute( CommandArgs args ); 
	}
}
