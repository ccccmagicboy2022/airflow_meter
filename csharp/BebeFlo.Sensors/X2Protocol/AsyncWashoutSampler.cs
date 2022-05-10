using System;
using System.Collections.Generic;
using BebeFlo.Utils;

namespace BebeFlo.Sensors.X2Protocol
{
	public class AsyncWashoutSampler : AsyncSamplerBase<WashoutRecord>
	{
		public AsyncWashoutSampler(X2Controller ctrl, double? ambientPressureHPa, int samplingFrequency, double callbackFrequency, Func<IList<WashoutRecord>, bool> dataCb, ExBackgroundWorker.AsyncErrorOccurredHandler errorCb, Func<X2ProtocolException, bool> dataStreamErrorHandler) : base(delegate(Func<IList<WashoutRecord>, bool> cb)
		{
         //采样函数
		ctrl.WashoutSampling(ambientPressureHPa, samplingFrequency, callbackFrequency, (IList<WashoutRecord> vals) => cb(vals), dataStreamErrorHandler);
		}, (IList<WashoutRecord> vals) => dataCb(vals), errorCb)
        //函数体
		{
			if (ctrl == null)
			{
				throw new ArgumentNullException("ctrl");
			}
			if (dataCb == null)
			{
				throw new ArgumentNullException("dataCb");
			}
		}
	}
}
