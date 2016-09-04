using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Service
{
	public interface IVolumeManager
	{
		IEnumerable<IVolume> VolumeServices { get; }
		IVolume DefaultVolume { get; }

		IVolume GetByHash( string hash );
	}
}
