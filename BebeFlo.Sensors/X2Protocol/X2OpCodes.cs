using System;

namespace BebeFlo.Sensors.X2Protocol
{
	public class X2OpCodes
	{
        //传感器大小
		public X2SensorSize SensorSize
		{
			get
			{
				return (X2SensorSize)(this.LSB & 3); //按位与0B11,返回0B00,0b01,0b10,0b11

            }
		}

		public int MSB;

		public int LSB;

		public int XSB;
	}
}
