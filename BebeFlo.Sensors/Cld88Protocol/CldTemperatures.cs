using System;
using BebeFlo.Utils;

namespace BebeFlo.Sensors.Cld88Protocol
{
	public class CldTemperatures
	{
		public double InstrumentInternal
		{
			get;
			set;
		}

		public double PeltierCooling
		{
			get;
			set;
		}

		public double ReactionChamber
		{
			get;
			set;
		}

		public double Converter
		{
			get;
			set;
		}

		public double HotTubing
		{
			get;
			set;
		}

		public double OzoneDestroyer
		{
			get;
			set;
		}

		public double OzoneGenerator
		{
			get;
			set;
		}

		public double VacuumPump
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
