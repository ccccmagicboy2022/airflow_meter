using System;
using System.Collections.Generic;
using BebeFlo.Utils;

namespace BebeFlo.Sensors.X2Protocol
{
	public class AsyncFlowNOSampler : AsyncSamplerBase<FlowNORecord>
	{
		public AsyncFlowNOSampler(X2Controller ctrl, int samplingFrequency, double callbackFrequency, Func<IList<FlowNORecord>, bool> dataCb, ExBackgroundWorker.AsyncErrorOccurredHandler errorCb) : base(delegate(Func<IList<FlowNORecord>, bool> cb)
		{
			ctrl.FlowNOSampling(samplingFrequency, callbackFrequency, (IList<FlowNORecord> vals) => cb(vals));
		}, dataCb, errorCb)
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
