using System;

namespace BebeFlo.Sensors.OxigrafProtocol
{
	public class OxigrafStatus
	{
		public OxigrafStatus(byte msb, byte lsb)
		{
			this._statusBytes = (ushort)(((int)msb << 8) + (int)lsb);
		}

		public bool NonZeroStandby
		{
			get
			{
				return this.ISBitSet(0);
			}
		}

		public bool SystemReady
		{
			get
			{
				return this.ISBitSet(1);
			}
		}

		public bool LaserTemperaturStabilized
		{
			get
			{
				return this.ISBitSet(2);
			}
		}

		public bool PartialPressureOnAnalogPort
		{
			get
			{
				return this.ISBitSet(3);
			}
		}

		public bool InvalidCalibrationFactors
		{
			get
			{
				return this.ISBitSet(4);
			}
		}

		public bool TestFaultWarning
		{
			get
			{
				return this.ISBitSet(5);
			}
		}

		public bool CellWarmingUp
		{
			get
			{
				return this.ISBitSet(6);
			}
		}

		public bool PressureCalibrationActive
		{
			get
			{
				return this.ISBitSet(7);
			}
		}

		public bool MicrocontrollerMemoryChecksumFailure
		{
			get
			{
				return this.ISBitSet(8);
			}
		}

		public bool ConfigEEPROMChecksumFailure
		{
			get
			{
				return this.ISBitSet(9);
			}
		}

		public bool WatchdogTimeOut
		{
			get
			{
				return this.ISBitSet(10);
			}
		}

		public bool InvalidO2ComputationError
		{
			get
			{
				return this.ISBitSet(11);
			}
		}

		public bool LowReferenceSignalLevel
		{
			get
			{
				return this.ISBitSet(12);
			}
		}

		public bool CellNullBalanceFailure
		{
			get
			{
				return this.ISBitSet(13);
			}
		}

		public bool LaserTemperatureControlFailure
		{
			get
			{
				return this.ISBitSet(14);
			}
		}

		private bool ISBitSet(ushort bitnr)
		{
			if (bitnr > 15)
			{
				throw new ArgumentOutOfRangeException("bitnr");
			}
			return ((int)this._statusBytes & 1 << (int)bitnr) != 0;
		}

		private ushort _statusBytes;
	}
}
