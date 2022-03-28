using System;
using System.Threading;
using BebeFlo.Sensors.Cld88Protocol;
using BebeFlo.Sensors.X2Protocol;

namespace BebeFlo.Sensors.OxigrafProtocol
{
	public class OxigrafController : DeviceController
	{
		public OxigrafController(ISensorPort serial) : base(serial, ExternalDevices.Oxigraf)
		{
		}

		public override void SetupDevice(double? ambientPressureHPa)
		{
			base.LockedExecution(delegate
			{
				int num = 10;
				bool flag = false;
				while (num > 0 && !flag)
				{
					flag = this.SystemReady();
					Thread.Sleep(150);
					num--;
				}
				if (!flag)
				{
					throw new OxigrafProtocolException("Oxigraf not ready");
				}
				this.ReportParameters(new char[]
				{
					'1'
				});
				this.ReportPeriod('2');
			});
		}

		public override void StopDevice()
		{
			base.LockedExecution(delegate
			{
				this.ReportPeriod('0');
			});
		}

		public bool SystemReady()
		{
			return this.GetStatus().SystemReady;
		}

		public OxigrafStatus GetStatus()
		{
			return base.LockedExecution<OxigrafStatus>(delegate
			{
				this.SetBinaryResponseFormat(true);
				OxigrafResponse oxigrafResponse = this.ReportParameterOnce(OxigrafParameter.SystemStatus);
				return new OxigrafStatus(oxigrafResponse.Data[0], oxigrafResponse.Data[1]);
			});
		}

		public OxigrafIdentificationReport IdentificationReport()
		{
			return base.LockedExecution<OxigrafIdentificationReport>(delegate
			{
				char reportPeriod = this._reportPeriod;
				this.ReportPeriod('0');
				this.SendMessage('W', new char[0]);
				OxigrafResponse oxigrafResponse = this.ReadReply();
				this.CheckResponse(oxigrafResponse, 'W');
				this.ReportPeriod(reportPeriod);
				if (oxigrafResponse.Data.Length != 6)
				{
					throw new OxigrafProtocolException(string.Format("6 data bytes expected for cmd 'W'. Got {0}", oxigrafResponse.Data.Length));
				}
				return new OxigrafIdentificationReport
				{
					SerialNumber = X2Utils.TwoComplementSignedSignal(((int)oxigrafResponse.Data[0] << 8) + (int)oxigrafResponse.Data[1], 16).ToString(),
					CellSerialNumber = X2Utils.TwoComplementSignedSignal(((int)oxigrafResponse.Data[2] << 8) + (int)oxigrafResponse.Data[3], 16).ToString(),
					OperatingTimeHours = X2Utils.TwoComplementSignedSignal(((int)oxigrafResponse.Data[4] << 8) + (int)oxigrafResponse.Data[5], 16)
				};
			});
		}

		public OxigrafMeasurementReport GetMeasurementReport()
		{
			return base.LockedExecution<OxigrafMeasurementReport>(delegate
			{
				OxigrafMeasurementReport oxigrafMeasurementReport = new OxigrafMeasurementReport();
				OxigrafResponse response = this.ReportParameterOnce(OxigrafParameter.OxygenConcentration);
				oxigrafMeasurementReport.O2Concentration = (double)this.NumericResponseValue(response) * 0.01;
				response = this.ReportParameterOnce(OxigrafParameter.SampleCellPressure);
				oxigrafMeasurementReport.O2SampleCellPressure = (double)this.NumericResponseValue(response) * 0.1;
				response = this.ReportParameterOnce(OxigrafParameter.SampleCellTemperature);
				oxigrafMeasurementReport.O2SampleCellTemperature = (double)this.NumericResponseValue(response) * 0.01;
				return oxigrafMeasurementReport;
			});
		}

		private OxigrafResponse ReportParameters(char[] parameters)
		{
			this.SendMessage('R', parameters);
			OxigrafResponse oxigrafResponse = this.ReadReply();
			this.CheckResponse(oxigrafResponse, 'R');
			return oxigrafResponse;
		}

		private OxigrafResponse ReportParameterOnce(OxigrafParameter parameter)
		{
			char reportPeriod = this._reportPeriod;
			this.ReportPeriod('0');
			this.SendMessage('L', (char)parameter);
			OxigrafResponse oxigrafResponse = this.ReadReply();
			this.CheckResponse(oxigrafResponse, 'L');
			this.ReportPeriod(reportPeriod);
			return oxigrafResponse;
		}

		private void SetBinaryResponseFormat(bool option)
		{
			this.ReportPeriod('0');
			char arg;
			if (option)
			{
				arg = '1';
			}
			else
			{
				arg = '0';
			}
			this.SendMessage('F', arg);
			this.CheckResponse('F');
		}

		private void ReportPeriod(char timeIn10ms)
		{
			this.SendMessage('P', timeIn10ms);
			this._reportPeriod = timeIn10ms;
		}

		private void SelfTest()
		{
			this.SendMessage('T', new char[0]);
		}

		private void SelfTest(char option)
		{
			this.SendMessage('T', option);
		}

		private void CheckResponse(char cmd)
		{
			this.ReadReply().ValidResponse(cmd);
		}

		private void CheckResponse(OxigrafResponse response, char cmd)
		{
			response.ValidResponse(cmd);
		}

		private void SendMessage(char cmd, char arg)
		{
			this.SendMessage(cmd, new char[]
			{
				arg
			});
		}

		private void SendMessage(char cmd, char[] args)
		{
			int count = 0;
			this._outbuf[count++] = 27;
			this._outbuf[count++] = (byte)cmd;
			for (int i = 0; i < args.Length; i++)
			{
				char c = args[i];
				this._outbuf[count++] = (byte)c;
			}
			this._outbuf[count++] = 59;
			this._serial.Write(this._outbuf, count);
		}

		private OxigrafResponse ReadReply()
		{
			this._serial.Read(this._inbuf, 0, 2);
			OxigrafResponse oxigrafResponse = new OxigrafResponse(this._inbuf[0], this._inbuf[1]);
			this._serial.Read(this._inbuf, 0, (int)(oxigrafResponse.Length + 2));
			if (oxigrafResponse.Length > 0)
			{
				oxigrafResponse.Command = this._inbuf[0];
				oxigrafResponse.Data = new byte[(int)(oxigrafResponse.Length - 1)];
				Array.Copy(this._inbuf, 1, oxigrafResponse.Data, 0, (int)(oxigrafResponse.Length - 1));
			}
			Array.Copy(this._inbuf, (int)oxigrafResponse.Length, oxigrafResponse.Checksum, 0, 2);
			return oxigrafResponse;
		}

		private int NumericResponseValue(OxigrafResponse response)
		{
			if (response.Data.Length != 2)
			{
				throw new OxigrafProtocolException(string.Format("2 byte numeric response expected. Got {0} bytes.", response.Data.Length));
			}
			return X2Utils.TwoComplementSignedSignal(((int)response.Data[0] << 8) + (int)response.Data[1], 16);
		}

		private readonly byte[] _outbuf = new byte[40];

		private readonly byte[] _inbuf = new byte[100];

		private char _reportPeriod = '0';
	}
}
