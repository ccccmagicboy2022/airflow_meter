using System;
using BebeFlo.Utils;

namespace BebeFlo.Sensors.Cld88Protocol
{
	public class CldStatus
	{
		public CldOperationMode OperationMode
		{
			get;
			set;
		}

		public bool InRemoteMode
		{
			get;
			set;
		}

		public CldComponentState ComponentStatus
		{
			get;
			set;
		}

		public CldInstrumentError ErrorStatus
		{
			get;
			set;
		}

		public CldInstrumentWarning WarningStatus
		{
			get;
			set;
		}

		public override string ToString()
		{
			return ReflectionHelper.DbgDumpPublicAttributes(this);
		}
	}
}
