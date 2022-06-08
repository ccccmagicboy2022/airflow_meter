using System;
using System.Globalization;
using BebeFlo.Sensors.CapnostatProtocol;
using BebeFlo.Sensors.Cld88Protocol;
using BebeFlo.Sensors.OxigrafProtocol;
using BebeFlo.Sensors.X2Protocol;

namespace BebeFlo.Sensors
{
	public class DeviceStatusRecord : IDumpable
	{
		public string ToDumpString(string separator)
		{
			string[] value = new string[]
			{
				DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo),
				(this.X2Versions != null) ? this.X2Versions.FirmwareString : string.Empty,
				(this.X2Versions != null) ? this.X2Versions.X2SerialNoString : string.Empty,
				this.EnvironmentRoomTemp.ToString(NumberFormatInfo.InvariantInfo),
				this.EnvironmentCaseTemp.ToString(NumberFormatInfo.InvariantInfo),
				this.EnvironmentPressure.ToString(NumberFormatInfo.InvariantInfo),
				(this.ASMainVersions != null) ? this.ASMainVersions.FirmwareString : string.Empty,
				(this.ASMainVersions != null) ? this.ASMainVersions.ASHardwareString : string.Empty,
				(this.ASMainVersions != null) ? this.ASMainVersions.ASSerialNoString : string.Empty,
				this.WashoutFlow.ToString(NumberFormatInfo.InvariantInfo),
				this.WashoutMMms.ToString(NumberFormatInfo.InvariantInfo),
				(this.ASMainFlowCalibration != null) ? this.ASMainFlowCalibration.TransitTimeBase.ToString(NumberFormatInfo.InvariantInfo) : string.Empty,
				(this.ASMainFlowCalibration != null) ? this.ASMainFlowCalibration.FlowGainInspiration.ToString(NumberFormatInfo.InvariantInfo) : string.Empty,
				(this.ASMainFlowCalibration != null) ? this.ASMainFlowCalibration.FlowGainExpiration.ToString(NumberFormatInfo.InvariantInfo) : string.Empty,
				this.SuitableDsrType,
				((int)(X2Utils.High(this.ASMainAmplitude) * 10)).ToString(NumberFormatInfo.InvariantInfo),
				((int)(X2Utils.Low(this.ASMainAmplitude) * 10)).ToString(NumberFormatInfo.InvariantInfo),
				(this.CldStatus != null) ? this.CldStatus.OperationMode.ToString() : string.Empty,
				(this.CldStatus != null) ? this.CldStatus.ErrorStatus.ToString() : string.Empty,
				(this.CldStatus != null) ? this.CldStatus.WarningStatus.ToString() : string.Empty,
				(this.CldCounts != null) ? this.CldCounts.ChannelA.ToString(NumberFormatInfo.InvariantInfo) : string.Empty,
				(this.CldTemperatures != null) ? this.CldTemperatures.OzoneDestroyer.ToString(NumberFormatInfo.InvariantInfo) : string.Empty,
				(this.CldTemperatures != null) ? this.CldTemperatures.ReactionChamber.ToString(NumberFormatInfo.InvariantInfo) : string.Empty,
				(this.CldTemperatures != null) ? this.CldTemperatures.PeltierCooling.ToString(NumberFormatInfo.InvariantInfo) : string.Empty,
				(this.CldTemperatures != null) ? this.CldTemperatures.InstrumentInternal.ToString(NumberFormatInfo.InvariantInfo) : string.Empty,
				(this.CldPressures != null) ? this.CldPressures.ReactionChamber.ToString(NumberFormatInfo.InvariantInfo) : string.Empty,
				(this.CldPressures != null) ? this.CldPressures.TubeType.ToString(NumberFormatInfo.InvariantInfo) : string.Empty,
				(this.CldFilterConfig != null) ? this.CldFilterConfig.ChannelBStatusString : string.Empty,
				(this.CldOperatingTime != null) ? this.CldOperatingTime.AnalyzerHours.ToString(NumberFormatInfo.InvariantInfo) : string.Empty,
				(this.CldCalibration != null) ? this.CldCalibration.Zero.ToString(NumberFormatInfo.InvariantInfo) : string.Empty,
				(this.CldCalibration != null) ? ((this.CldCalibration.Slope != 0.0) ? (1.0 / this.CldCalibration.Slope).ToString(NumberFormatInfo.InvariantInfo) : double.PositiveInfinity.ToString()) : string.Empty,
				this.CldSampleGasFlow.ToString(NumberFormatInfo.InvariantInfo),
				(this.ASSideVersions != null) ? this.ASSideVersions.FirmwareString : string.Empty,
				(this.ASSideVersions != null) ? this.ASSideVersions.ASHardwareString : string.Empty,
				(this.ASSideVersions != null) ? this.ASSideVersions.ASSerialNoString : string.Empty,
				this.WashoutSampleFlow.ToString(NumberFormatInfo.InvariantInfo),
				this.WashoutMMss.ToString(NumberFormatInfo.InvariantInfo),
				(this.CapnostatProperties != null) ? this.CapnostatProperties.SleepMode.ToString() : string.Empty,
				this.CapnostatStatus.ToString(),
				this.WashoutCO2.ToString(NumberFormatInfo.InvariantInfo),
				(this.CapnostatProperties != null) ? this.CapnostatProperties.CO2Unit.ToString() : string.Empty,
				this.CO2Gain.ToString(NumberFormatInfo.InvariantInfo),
				this.CO2Offset.ToString(NumberFormatInfo.InvariantInfo),
				(this.CapnostatProperties != null) ? this.CapnostatProperties.TotalUseTimeMinutes.ToString(NumberFormatInfo.InvariantInfo) : string.Empty,
				(this.CapnostatProperties != null) ? this.CapnostatProperties.LastZeroTimeMinutes.ToString(NumberFormatInfo.InvariantInfo) : string.Empty,
				(this.CapnostatProperties != null) ? this.CapnostatProperties.SensorSerialNumber : string.Empty,
				(this.CapnostatProperties != null) ? this.CapnostatProperties.SensorPartNumber : string.Empty,
				(this.OxigrafStatus != null) ? this.OxigrafStatus.SystemReady.ToString() : string.Empty,
				(this.OxigrafStatus != null) ? this.OxigrafStatus.LaserTemperaturStabilized.ToString() : string.Empty,
				this.WashoutO2.ToString(NumberFormatInfo.InvariantInfo),
				(this.OxigrafMeasurements != null) ? this.OxigrafMeasurements.O2SampleCellPressure.ToString(NumberFormatInfo.InvariantInfo) : string.Empty,
				(this.OxigrafMeasurements != null) ? this.OxigrafMeasurements.O2SampleCellTemperature.ToString(NumberFormatInfo.InvariantInfo) : string.Empty,
				this.O2Gain.ToString(NumberFormatInfo.InvariantInfo),
				this.O2Offset.ToString(NumberFormatInfo.InvariantInfo),
				(this.OxigrafIdentification != null) ? this.OxigrafIdentification.OperatingTimeHours.ToString(NumberFormatInfo.InvariantInfo) : string.Empty,
				(this.OxigrafIdentification != null) ? this.OxigrafIdentification.SerialNumber : string.Empty,
				(this.OxigrafIdentification != null) ? this.OxigrafIdentification.CellSerialNumber : string.Empty
			};
			return string.Join(separator, value);
		}

		public string[] GetHeaders()
		{
			return new string[]
			{
				"DateTime",
				"X2 Firmware-Version",
				"X2 Serial number",
				"X2 Room temperature [°C]",
				"X2 Case temperature [°C]",
				"X2 Pressure [hPa]",
				"Main-AS Firmware-Version",
				"Main-AS Hardware-Version",
				"Main-AS Serial number",
				"Main-AS Flow [ml/s]",
				"Main-AS MMms [g/mol]",
				"Main-AS TT Base",
				"Main-AS Flow gain (In)",
				"Main-AS Flow gain (Ex)",
				"Main-AS Suitable for DSR",
				"Main-AS Amplitude channel 1 [mV]",
				"Main-AS Amplitude channel 2 [mV]",
				"CLD Operation mode",
				"CLD Error status",
				"CLD Warning status",
				"CLD Counts",
				"CLD Scrubber temperature [°C]",
				"CLD Reactor temperature [°C]",
				"CLD Peltier temperature [°C]",
				"CLD Instrument temperature [°C]",
				"CLD Reactor pressure [hPa]",
				"CLD Sample tube type",
				"CLD Filter",
				"CLD Operating time [h]",
				"CLD NO zero [cps]",
				"CLD NO slope [cps/ppb]",
				"CLD Sample gas flow [ml/min]",
				"Side-AS Firmware-Version",
				"Side-AS Hardware-Version",
				"Side-AS Serial number",
				"Side-AS Sample Flow [ml/min]",
				"Side-AS MMss [g/mol]",
				"Capnostat Sleep mode",
				"Capnostat Prioritized status",
				"Capnostat CO2 [%]",
				"Capnostat CO2 unit",
				"Capnostat CO2 gain",
				"Capnostat CO2 offset",
				"Capnostat Total use time [min]",
				"Capnostat Last Zero [min]",
				"Capnostat Serial no.",
				"Capnostat Sensor part no.",
				"Oxigraf System ready",
				"Oxigraf O2 sensor temperature stable",
				"Oxigraf O2 [%]",
				"Oxigraf O2 sample cell pressure [hPa]",
				"Oxigraf O2 sample cell temperature [°C]",
				"Oxigraf O2 gain",
				"Oxigraf O2 offset",
				"Oxigraf Operating time [h]",
				"Oxigraf Serial no.",
				"Oxigraf Cell serial no."
			};
		}

		public X2Versions X2Versions;

		public double EnvironmentCaseTemp;

		public double EnvironmentRoomTemp;

		public double EnvironmentPressure;

		public X2Versions ASMainVersions;

		public float WashoutFlow;

		public float WashoutMMms;

		public X2FlowCalibration ASMainFlowCalibration;

		public string SuitableDsrType;

		public ushort ASMainAmplitude;

		public CldStatus CldStatus;

		public CldCounts CldCounts;

		public CldTemperatures CldTemperatures;

		public CldPressures CldPressures;

		public CldFilterConfig CldFilterConfig;

		public CldOperatingTime CldOperatingTime;

		public CldCalibration CldCalibration;

		public double CldSampleGasFlow;

		public X2Versions ASSideVersions;

		public float WashoutSampleFlow;

		public float WashoutMMss;

		public CapnostatConstantProperties CapnostatProperties;

		public CapnostatPrioritizedStatus CapnostatStatus = CapnostatPrioritizedStatus.StatusNotReceived;

		public float WashoutCO2;

		public double CO2Gain;

		public double CO2Offset;

		public OxigrafStatus OxigrafStatus;

		public float WashoutO2;

		public OxigrafMeasurementReport OxigrafMeasurements;

		public double O2Gain;

		public double O2Offset;

		public OxigrafIdentificationReport OxigrafIdentification;
	}
}
